using AutoMapper;
using Common.BusinessObjects;
using Common.BusinessObjects.Interfaces;
using Common.DataAccess.Interfaces;
using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Common.Infrastructure.Extensions;
using Common.Infrastructure.Helpers;
using Common.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NambayaUser.Contracts.Interfaces;
using NambayaUser.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using UserManagement.Contracts.Models;
using UserBO = NambayaUser.Contracts.Models.UserBO;

namespace NambayaUser.Service
{
    public class NambayaUserService : BaseUserService, INambayaUserService
    {
        private readonly IMapper _mapper;
        private readonly WebServiceConfiguration _webApiConf;
        private readonly IUserRepository _nambayaUserRepository;
        private readonly ICommonUserServce _commonUserServce;
        private readonly IConfiguration _configuration;
        private readonly ILogger<NambayaUserService> _logger;

        public NambayaUserService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _nambayaUserRepository = serviceProvider.GetRequiredService<IUserRepository>();
            _mapper = serviceProvider.GetRequiredService<IMapper>();
            _webApiConf = serviceProvider.GetRequiredService<WebServiceConfiguration>();
            _commonUserServce = serviceProvider.GetRequiredService<ICommonUserServce>();
            _configuration = serviceProvider.GetRequiredService<IConfiguration>();
            _logger = serviceProvider.GetRequiredService<ILogger<NambayaUserService>>();
        }

        public PagedResults<UserBO> GetUsers(int qLimit, int offset, string qOrderby, string param)
        {
            _logger.LogInformation($"GetUsers: qLimit {qLimit} offset {offset} qOrderby{qOrderby} param {param}");
            PagedResults<UserBO> pg = new PagedResults<UserBO>();
            bool? isActiveFilter = null;
            bool? isLockedFilter = null;
            _logger.LogInformation("GetUsers: creating filters");
            IFilter filter = GetUsersDefaultFilter(param, ref isActiveFilter, ref isLockedFilter);
            var orderby = getAdjustedOrderBy(qOrderby, isActiveFilter, isLockedFilter);
            int limit = 0;// getAdjustedQueryLimit(qOrderby, qLimit, isActiveFilter, isLockedFilter);
            var pdb = _nambayaUserRepository.GetAllPages(limit, offset, orderby, filter);
            pg.TotalCount = pdb.TotalCount;
            pg.PageSize = pdb.PageSize;
            _logger.LogInformation($"GetUsers: Filter TotalCount: {pg.TotalCount} PageSize: {pg.PageSize}");
            pg.Data = pdb.Data.Select(p => _mapper.Map<UserBO>(p)).ToList();
            pg.Data.ToList().ForEach(p => p.LoginName = p.Email);
            pg.Data.ToList().ForEach(p => p.ApplicationCode = ApplicationNames.NamabayaUserApp);
            GetUMData(_webApiConf.UserManagementServiceBaseUrl, GetRoleCodes(param), pg, qOrderby,
                qLimit, limit, offset, isActiveFilter, isLockedFilter);
            _logger.LogInformation("GetUsers: completed");

            return pg;
        }
        protected override IFilter GetUsersDefaultFilter(string param, ref bool? isActiveFilter, ref bool? isLockedFilter)
        {
            IFilter filter = null;
            _logger.LogInformation($"GetUsersDefaultFilter: with params {param}");
            if (!string.IsNullOrWhiteSpace(param))
            {
                var expressions = Expression.StringToExpressions(param);
                expressions = expressions.Where(p => !p.Property.ToLower().StartsWith("rolecode"));
                if (expressions.Where(p => p.Property.ToLower() == "name").Any())
                    expressions.Where(p => p.Property.ToLower() == "name").FirstOrDefault().Property = "FirstName +' '+LastName";
                var filterlist = expressions.Where(p => p.Property.ToLower() != "isactive" && p.Property.ToLower() != "islocked")
                    .Select(p => new Filter(p.Property, p.Operation, p.Value)).ToList();
                filter = new ANDFilter(filterlist.Select(p => p as IFilter).ToList());
                if (expressions.Any(p => p.Property.ToLower() == "isactive"))
                    isActiveFilter = expressions.FirstOrDefault(p => p.Property.ToLower() == "isactive").Value.ToString().ToLower() == "true";
                if (expressions.Any(p => p.Property.ToLower() == "islocked"))
                    isLockedFilter = expressions.FirstOrDefault(p => p.Property.ToLower() == "islocked").Value.ToString().ToLower() == "true";
            }
            return filter;
        }
        private string GetRoleCodes(string param)
        {
            _logger.LogInformation($"Getting role code with params {param}");
            if (!string.IsNullOrWhiteSpace(param))
            {
                var expressions = Expression.StringToExpressions(param);
                var roleVal = expressions.Where(p => p.Property.ToLower() == "rolecode").Select(p => p.Value).FirstOrDefault();
                List<string> roleCodes = new List<string>();
                if (roleVal is JsonElement)
                {
                    if (((JsonElement)roleVal).ValueKind == JsonValueKind.Array)
                        roleCodes = ((JsonElement)roleVal).EnumerateArray().ToList().Select(p => p.ToString()).ToList();
                    else if (((JsonElement)roleVal).ValueKind == JsonValueKind.String)
                        roleCodes.Add(roleVal.ToString());
                    IList<string> strings = new List<string>();
                    foreach (var rolecode in roleCodes)
                    {
                        if (rolecode.ToString().ToLower() == RoleCodes.NambayaUser.ToLower())
                            strings.Add(RoleCodes.NambayaUser);
                        else if (rolecode.ToString().ToLower() == RoleCodes.StakeHolder.ToLower())
                            strings.Add(RoleCodes.StakeHolder);
                        else if (rolecode.ToString().ToLower() == RoleCodes.PharmacyTrainer.ToLower())
                            strings.Add(RoleCodes.PharmacyTrainer);
                    }
                    if (strings.Count > 0)
                        return string.Join(",", strings);
                }
            }
            return $"{RoleCodes.NambayaUser},{RoleCodes.PharmacyTrainer},{RoleCodes.StakeHolder}";
        }
        public UserBO GetUserById(long id)
        {
            _logger.LogInformation($"GetUserById: id {id}");
            var user = _mapper.Map<UserBO>(_nambayaUserRepository.GetByID(id));
            if (user != null)
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync("api/v1/user/loginname/" + user.Email).Result;
                    var b = JsonSerializer.Deserialize<BaseUserBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    user.ApplicationCode = ApplicationNames.NamabayaUserApp;
                    user.Role = b.Role;
                    user.LoginName = b.LoginName;
                    user.IsActive = b.IsActive;
                    user.IsLocked = b.IsLocked;
                    user.IsPasswordResetRequired = b.IsPasswordResetRequired;
                    user.Role = GetRole(user.Email).Code;
                }
            }
            else
            {
                _logger.LogInformation($"GetUserById: user not found ");
            }
            return user;
        }
        public UserBO GetUserByEmail(string email)
        {
            _logger.LogInformation($"GetUserByEmail: email: {email}");
            var user = _mapper.Map<UserBO>(_nambayaUserRepository.GetByEmail(email));
            if (user != null)
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync("api/v1/user/loginname/" + user.Email).Result;
                    var b = JsonSerializer.Deserialize<BaseUserBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    user.ApplicationCode = ApplicationNames.NamabayaUserApp;
                    user.Role = b.Role;
                    user.LoginName = b.LoginName;
                    user.IsActive = b.IsActive;
                    user.IsLocked = b.IsLocked;
                    user.IsPasswordResetRequired = b.IsPasswordResetRequired;
                    user.Role = GetRole(user.Email).Code;
                }
            }
            else
            {
                _logger.LogInformation($"GetUserByEmail: user not found ");
            }

            return user;
        }
        public long GetTotal()
        {
            return _nambayaUserRepository.GetCount();
        }
        public long AddUser(UserBO userBO)
        {
            _logger.LogInformation($"AddUser: user {Newtonsoft.Json.JsonConvert.SerializeObject(userBO)}");
            using (var unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                try
                {
                    var sessionContext = GetSessionContext();
                    if (sessionContext.RoleCode != RoleCodes.NambayaUser)
                        throw new ServiceException(ClientSideErrors.USER_NOT_AUTHORIZED);
                    var p = _mapper.Map<Repository.Models.User>(userBO);
                    /*if (userBO.Role == RoleCodes.NambayaUser)
                    {
                        var allowedDomain = _configuration.GetSection("NambayaUserAllowedUserDomains").Get<string[]>();
                        bool isValid = false;
                        foreach (var domain in allowedDomain)
                        {
                            if (userBO.Email.ToLower().EndsWith(domain.ToLower()))
                                isValid = true;
                        }
                        if (!isValid)
                            throw new ServiceException(ClientSideErrors.INVALID_USER_APPLICATION_ROLE);
                    }*/
                    using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                    {
                        _logger.LogInformation("Calling user api for user creation");
                        var res = apiClient.InternalServicePostAsync("api/v1/user/Create", JsonSerializer.Serialize(
                            new BaseUserBO()
                            {
                                ApplicationCode = ApplicationNames.NamabayaUserApp,
                                LoginName = userBO.Email,
                                Role = userBO.Role,
                                Password = userBO.Password,
                                IsActive = userBO.IsActive,
                                IsLocked = userBO.IsLocked,
                                CreatedBy = sessionContext?.LoginName
                            })).Result;
                    }
                    p.PhoneVerified = false;
                    p.CreatedBy = sessionContext?.LoginName;
                    _nambayaUserRepository.Insert(p);
                    unitOfWork.Commit();
                    _logger.LogInformation("AddUser: Completed");
                    return p.ID;
                }
                catch (AggregateException ex)
                {
                    unitOfWork.Rollback();
                    throw ex.InnerException;
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }
            }
        }

        public void UpdateUser(UserBO userBO)
        {
            _logger.LogInformation($"UpdateUser: user id {userBO.ID}");
            using (var unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                try
                {
                    var sessionContext = GetSessionContext();
                    var p = _mapper.Map<Repository.Models.User>(userBO);
                    var dbUser = _nambayaUserRepository.GetByID(userBO.ID);
                    using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                    {
                        var res = apiClient.InternalServicePostAsync("api/v1/user/Update", JsonSerializer.Serialize(
                            new BaseUserBO()
                            {
                                ApplicationCode = ApplicationNames.NamabayaUserApp,
                                Role = userBO.Role,
                                LoginName = userBO.Email,
                                Password = userBO.Password,
                                IsActive = userBO.IsActive,
                                IsLocked = userBO.IsLocked,
                                UpdatedBy = sessionContext?.LoginName
                            })).Result;
                    }
                    p.PhoneVerified = dbUser.PhoneVerified;
                    if (p.Phone != dbUser.Phone)
                    {
                        _logger.LogInformation($"Phone number change.. Reverification required");
                        p.PhoneVerified = false;
                        _commonUserServce.UpdateSettings(p.Email, new List<UserSettingBO>()
                        {
                            new UserSettingBO()
                            {
                                Code = UserSettingCodes.FACTOR_NOTIFICATION_TYPE,
                                DataType = UserSettingDataTypes.LOOKUPS,
                                Value = NotificationType.Email.ToString()
                            }
                        });
                    }
                    p.UpdatedBy = sessionContext?.LoginName;
                    _nambayaUserRepository.Update(p);
                    _logger.LogInformation($"Update user: completed");
                    unitOfWork.Commit();
                }
                catch //(Exception e)
                {
                    unitOfWork.Rollback();
                    throw;
                }
            }
        }

        public void DeleteUser(UserBO userBO)
        {
            var p = _mapper.Map<Repository.Models.User>(userBO);
            _nambayaUserRepository.Delete(p);
        }

        private RoleObjectBO GetRole(string email)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync("api/v1/role/loginname/" + email).Result;
                    return JsonSerializer.Deserialize<RoleObjectBO>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
        }
        public object GetRoles(string applicationCode)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/role/{applicationCode}").Result;
                    return JsonSerializer.Deserialize<IEnumerable<RoleBO>>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
        }
        public object GetStats()
        {
            try
            {
                _logger.LogInformation($"GetStats started");
                long totalCardios, totalPharmas, totalCenterUsers;
                var sessioncontext = GetSessionContext();
                if (sessioncontext.RoleCode != RoleCodes.NambayaUser && sessioncontext.RoleCode != RoleCodes.PharmacyTrainer && sessioncontext.RoleCode != RoleCodes.StakeHolder)
                    throw new ServiceException(ClientSideErrors.INVALID_USER_APPLICATION_ROLE);
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PharmacistServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/pharmacy/total").Result;
                    totalPharmas = JsonSerializer.Deserialize<long>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.CardiologistServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/cardiologist/total").Result;
                    totalCardios = JsonSerializer.Deserialize<long>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.CentralGroupServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/centralgroupuser/total").Result;
                    totalCenterUsers = JsonSerializer.Deserialize<long>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
                return new
                {
                    totalUsers = GetTotal(),
                    totalCardios,
                    totalPharmas,
                    totalCenterUsers
                };
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public string GeneratePhoneVerification(VerifyPhoneRequest req)
        {
            SessionContext sessionContext = GetSessionContext();
            var user = _mapper.Map<UserBO>(_nambayaUserRepository.GetByEmail(sessionContext.LoginName));
            if (string.IsNullOrWhiteSpace(req.PhoneNumber))
                throw new ServiceException(ClientSideErrors.INVALID_USER_ACTION);
            var rabbitmq = _serviceProvider.GetRequiredService<RabbitMQClient>();
            int digit = 8;
            var otp = new Random().Next((int)Math.Pow(10, digit - 1) + 1, (int)Math.Pow(10, digit) - 1).ToString("########");
            var sid = DateTime.Now.Ticks.ToString();
            var hash = string.Format("{0}:{1}", sid, otp).Sha256();
            var token = JwtWrapper.GenerateToken(ConfigurationConsts.PhoneVerificationKey.Sha256(), new Claim[]
                {
                  new Claim("sid", sid),
                  new Claim("otp_email", user.Email),
                  new Claim("otp_phone", req.PhoneNumber??""),
                  new Claim("otp_name", user.Name??""),
                  new Claim("otp_hash", hash),
                }, 120);
            rabbitmq.SendMessage(KnownChannels.PHONE_VERIFICATION_EVENT_CHANNEL, new UserOtp()
            {
                AppUserID = user.ID,
                Email = user.Email,
                Name = user.Name,
                Phone = req.PhoneNumber,
                OTP = otp,
                SMS = true
            });
            return token;
        }
        public UserBO VerifyPhoneVerification(VerifyPhoneOtpRequest req)
        {
            try
            {
                SessionContext sessionContext = GetSessionContext();
                var user = _nambayaUserRepository.GetByEmail(req.Email);
                var principle = JwtWrapper.GetClaimsPrincipal(ConfigurationConsts.PhoneVerificationKey.Sha256(), req.Token);
                if (principle?.Identity?.IsAuthenticated == true)
                {
                    var claimsIdentity = principle.Identity as ClaimsIdentity;
                    var claims = claimsIdentity.Claims.ToArray();
                    var sid = claims.Single(c => c.Type == "sid").Value;
                    var hash = claims.Single(c => c.Type == "otp_hash").Value;
                    if (hash == string.Format("{0}:{1}", sid, req.OTP.ToString()).Sha256() &&
                        claims.Single(c => c.Type == "otp_email").Value == req.Email &&
                        claims.Single(c => c.Type == "otp_phone").Value == req.PhoneNumber)
                    {
                        user.PhoneVerified = true;
                        user.Phone = req.PhoneNumber;
                        user.UpdatedBy = sessionContext.LoginName;
                        _nambayaUserRepository.Update(user);
                        return _mapper.Map<UserBO>(user);
                    }
                }
            }
            catch
            {
            }
            throw new ServiceException(ClientSideErrors.OTP_VERIFICATION_FAILED);
        }
    }
}
