using AutoMapper;
using CentralGroup.Contracts.Interfaces;
using CentralGroup.Contracts.Models;
using CentralGroup.Repository.Interfaces;
using Common.BusinessObjects;
using Common.BusinessObjects.Interfaces;
using Common.DataAccess.Interfaces;
using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Common.Infrastructure.Extensions;
using Microsoft.Extensions.Logging;
using Common.Infrastructure.Helpers;
using Common.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace CentralGroup.Service
{
    public class CentralGroupService : BaseUserService, ICentralGroupService
    {
        private readonly ICentralGroupRepository _centralGroupRepository;
        private readonly IMapper _mapper;
        private readonly WebServiceConfiguration _webApiConf;
        private readonly ICommonUserServce _commonUserServce;
        private readonly ILogger<CentralGroupService> _logger;
        public CentralGroupService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _webApiConf = serviceProvider.GetRequiredService<WebServiceConfiguration>();
            _mapper = serviceProvider.GetRequiredService<IMapper>();
            _centralGroupRepository = serviceProvider.GetRequiredService<ICentralGroupRepository>();
            _commonUserServce = serviceProvider.GetRequiredService<ICommonUserServce>();
            _logger = serviceProvider.GetRequiredService<ILogger<CentralGroupService>>();
        }
        PagedResults<CentralGroupBO> ICentralGroupService.GetCentralGroup(int qLimit, int offset, string qOrderby, string param)
        {
            _logger.LogInformation($"Get Center group : limit {qLimit} offset {offset} orderby {qOrderby} param {param}");

            PagedResults<CentralGroupBO> pg = new PagedResults<CentralGroupBO>();
            bool? isActiveFilter = null;
            bool? isLockedFilter = null;
            _logger.LogInformation($"GetCentralGroup: Creating filter for getting central group ");
            IFilter filter = GetUsersDefaultFilter(param, ref isActiveFilter, ref isLockedFilter);
            var orderby = getAdjustedOrderBy(qOrderby, isActiveFilter, isLockedFilter);
            int limit = getAdjustedQueryLimit(qOrderby, qLimit, isActiveFilter, isLockedFilter);
            var pdb = _centralGroupRepository.GetAllPages(limit, offset, orderby, filter);
            pg.TotalCount = pdb.TotalCount;
            pg.PageSize = pdb.PageSize;
            _logger.LogInformation($"GetCentralGroup: TotalCount {pg.TotalCount} page size {pg.PageSize} ");

            pg.Data = pdb.Data.Select(p => _mapper.Map<CentralGroupBO>(p)).ToList();
            pg.Data.ToList().ForEach(p => p.LoginName = p.Email);
            pg.Data.ToList().ForEach(p => p.ApplicationCode = ApplicationNames.CentralGroupApp);
            GetUMData(_webApiConf.UserManagementServiceBaseUrl, RoleCodes.CentralGroupUser, pg, qOrderby,
                qLimit, limit, offset, isActiveFilter, isLockedFilter);
            _logger.LogInformation($"Sucessfully get data for center user");
            return pg;
        }
        CentralGroupBO ICentralGroupService.GetCentralGroupById(long id)
        {
            _logger.LogInformation($"GetCentralGroupById: center group by id {id}");
            var user = _mapper.Map<CentralGroupBO>(_centralGroupRepository.GetByID(id));
            if (user != null)
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync("api/v1/user/loginname/" + user.Email).Result;
                    var b = JsonSerializer.Deserialize<BaseUserBO>(res,
                       new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    user.ApplicationCode = ApplicationNames.CentralGroupApp;
                    user.Role = RoleCodes.CentralGroupUser;
                    user.LoginName = b.LoginName;
                    user.IsActive = b.IsActive;
                    user.IsLocked = b.IsLocked;
                    user.IsPasswordResetRequired = b.IsPasswordResetRequired;
                    user.Role = GetRole(user.Email).Code;
                }
            }
            else
            {
                _logger.LogInformation($"GetCentralGroupById: not found");
            }
            return user;
        }
        CentralGroupBO ICentralGroupService.GetCentralGroupByEmail(string email)
        {
            _logger.LogInformation($"GetCentralGroupByEmail: email {email}");

            var user = _mapper.Map<CentralGroupBO>(_centralGroupRepository.GetByEmail(email));
            if (user != null)
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync("api/v1/user/loginname/" + user.Email).Result;
                    var b = JsonSerializer.Deserialize<BaseUserBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    user.ApplicationCode = ApplicationNames.CentralGroupApp;
                    user.Role = RoleCodes.CentralGroupUser;
                    user.LoginName = b.LoginName;
                    user.IsActive = b.IsActive;
                    user.IsLocked = b.IsLocked;
                    user.IsPasswordResetRequired = b.IsPasswordResetRequired;
                    user.Role = GetRole(user.Email).Code;
                }
            }
            else
            {
                _logger.LogInformation($"GetCentralGroupByEmail: not found");

            }
            return user;
        }
        long ICentralGroupService.GetTotal()
        {
            return _centralGroupRepository.GetCount();
        }
        long ICentralGroupService.AddCentralGroup(CentralGroupBO centralGroup)
        {
            _logger.LogInformation($"AddCentralGroup: center group {Newtonsoft.Json.JsonConvert.SerializeObject(centralGroup)}");
            var p = _mapper.Map<Repository.Models.CentralGroup>(centralGroup);
            using (var _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                try
                {
                    using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                    {
                        var res = apiClient.InternalServicePostAsync("api/v1/user/Create", JsonSerializer.Serialize(
                            new BaseUserBO()
                            {
                                ApplicationCode = ApplicationNames.CentralGroupApp,
                                LoginName = p.Email,
                                Password = centralGroup.Password,
                                Role = RoleCodes.CentralGroupUser,
                                IsActive = centralGroup.IsActive,
                                IsLocked = centralGroup.IsLocked,
                                CreatedBy = centralGroup.CreatedBy
                            })).Result;
                    }
                    p.PhoneVerified = false;
                    _centralGroupRepository.Insert(p);
                    _unitOfWork.Commit();
                    _logger.LogInformation($"Center group information added");
                    return p.ID;
                }
                catch (AggregateException ex)
                {
                    _unitOfWork.Rollback();
                    throw ex.InnerException;
                }
                catch //(Exception e)
                {
                    _unitOfWork.Rollback();
                    throw;
                }
            }
        }

        void ICentralGroupService.UpdateCentralGroup(CentralGroupBO centralGroup)
        {
            _logger.LogInformation($"UpdateCentralGroup: center group {Newtonsoft.Json.JsonConvert.SerializeObject(centralGroup)}");

            var p = _mapper.Map<Repository.Models.CentralGroup>(centralGroup);
            var dbUser = _centralGroupRepository.GetByID(centralGroup.ID);
            using (var _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                try
                {
                    using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                    {
                        var res = apiClient.InternalServicePostAsync("api/v1/user/Update", JsonSerializer.Serialize(
                            new BaseUserBO()
                            {
                                ApplicationCode = ApplicationNames.CentralGroupApp,
                                Role = RoleCodes.CentralGroupUser,
                                LoginName = centralGroup.Email,
                                Password = centralGroup.Password,
                                IsActive = centralGroup.IsActive,
                                IsLocked = centralGroup.IsLocked,
                                UpdatedBy = centralGroup.UpdatedBy,
                            })).Result;
                    }
                    p.PhoneVerified = dbUser.PhoneVerified;
                    if (p.Phone != dbUser.Phone)
                    {
                        _logger.LogInformation($"Phone number changed reverification required");
                        p.PhoneVerified = false;
                        _commonUserServce.UpdateSettings(p.Email, new List<UserSettingBO>()
                        {
                            new UserSettingBO()
                            {   Code = UserSettingCodes.FACTOR_NOTIFICATION_TYPE,
                                DataType = UserSettingDataTypes.LOOKUPS,
                                Value = NotificationType.Email.ToString()
                            }
                        });
                    }
                    _centralGroupRepository.Update(p);
                    _logger.LogInformation($"Sucessfully update the center group info");
                    _unitOfWork.Commit();
                }
                catch (AggregateException ex)
                {
                    throw ex.InnerException;
                }
                catch //(Exception ex)
                {
                    _unitOfWork.Rollback();
                    throw;
                }
            }
        }

        void ICentralGroupService.DeleteCentralGroup(CentralGroupBO centralGroup)
        {
            _logger.LogInformation("Delete central group started");
            var p = _mapper.Map<Repository.Models.CentralGroup>(centralGroup);
            _centralGroupRepository.Delete(p);
        }
        private RoleObjectBO GetRole(string email)
        {
            _logger.LogInformation($"Getting role for the email {email}");
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync("api/v1/role/loginname/" + email).Result;
                    return JsonSerializer.Deserialize<RoleObjectBO>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (AggregateException ex)
            {
                throw ex.InnerException;
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
        }
        public string GeneratePhoneVerification(VerifyPhoneRequest req)
        {
            _logger.LogInformation($"GeneratePhoneVerification: phone verification stated..");
            SessionContext sessionContext = GetSessionContext();
            var user = _mapper.Map<CentralGroupBO>(_centralGroupRepository.GetByEmail(sessionContext.LoginName));
            _logger.LogInformation($"Phone verification for user {Newtonsoft.Json.JsonConvert.SerializeObject(user)}");
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
            _logger.LogInformation("Sending message the phone verification event channel");
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
        public CentralGroupBO VerifyPhoneVerification(VerifyPhoneOtpRequest req)
        {
            _logger.LogInformation($"VerifyPhoneVerification: started ");
            try
            {
                SessionContext sessionContext = GetSessionContext();
                var user = _centralGroupRepository.GetByEmail(req.Email);
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
                        _logger.LogInformation("VerifyPhoneVerification: phone sucessfully verified");
                        user.PhoneVerified = true;
                        user.Phone = req.PhoneNumber;
                        user.UpdatedBy = sessionContext.LoginName;
                        _centralGroupRepository.Update(user);
                        return _mapper.Map<CentralGroupBO>(user);
                    }
                }
                else
                {
                    _logger.LogInformation("VerifyPhoneVerification: User identity is not verified");
                }
            }
            catch
            {
            }
            throw new ServiceException(ClientSideErrors.OTP_VERIFICATION_FAILED);
        }
    }
}
