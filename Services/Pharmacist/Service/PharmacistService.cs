using AutoMapper;
using Common.BusinessObjects;
using Common.BusinessObjects.Interfaces;
using Common.DataAccess.Interfaces;
using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Common.Infrastructure.Extensions;
using Common.Infrastructure.Helpers;
using Common.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pharmacist.Contracts.Interfaces;
using Pharmacist.Contracts.Models;
using Pharmacist.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace Pharmacist.Service
{
    public class PharmacistService : BaseUserService, IPharmacistService
    {
        private readonly IPharmacistRepository _pharmacistRepository;
        private readonly IMapper _mapper;
        private readonly WebServiceConfiguration _webApiConf;
        private readonly ICommonUserServce _commonUserServce;
        private readonly ILogger<PharmacistService> _logger;
        public PharmacistService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _webApiConf = serviceProvider.GetRequiredService<WebServiceConfiguration>();
            _mapper = serviceProvider.GetRequiredService<IMapper>();
            _pharmacistRepository = serviceProvider.GetRequiredService<IPharmacistRepository>();
            _commonUserServce = serviceProvider.GetRequiredService<ICommonUserServce>();
            _logger = serviceProvider.GetRequiredService<ILogger<PharmacistService>>();
        }
        PagedResults<PharmacistBO> IPharmacistService.GetPharmacists(int qLimit, int offset, string qOrderby, string param)
        {
            _logger.LogInformation($"GetPharmacists: qLimit {qLimit} offset {offset} qOrderby {qOrderby} param {param}");
            var sessioncontext = GetSessionContext();
            if (sessioncontext.RoleCode != RoleCodes.Pharmacy)
                throw new ServiceException(ClientSideErrors.USER_NOT_AUTHORIZED);
            var pharmacyid = sessioncontext.AppUserID;
            PagedResults<PharmacistBO> pg = new PagedResults<PharmacistBO>();
            bool? isActiveFilter = null;
            bool? isLockedFilter = null;
            _logger.LogInformation("GetPharmacists: Creating filters");
            IFilter filter = GetUsersDefaultFilter(param, ref isActiveFilter, ref isLockedFilter);
            IFilter pharmacyFilter = new ANDFilter(new List<IFilter>() { new Filter(strFilterName: "pharmacyid", sqlOperator: SqlOperators.Equal, strFilterValue: pharmacyid) });
            if (filter == null)
            {
                _logger.LogInformation("User default filter is null setting pharmacy filter");
                filter = pharmacyFilter;
            }         
            else
                filter = new ANDFilter(filter, pharmacyFilter);
            var orderby = getAdjustedOrderBy(qOrderby, isActiveFilter, isLockedFilter);
            int limit = getAdjustedQueryLimit(qOrderby, qLimit, isActiveFilter, isLockedFilter);
            var pdb = _pharmacistRepository.GetAllPages(limit, offset, orderby, filter);
            pg.TotalCount = pdb.TotalCount;
            pg.PageSize = pdb.PageSize;
            pg.Data = pdb.Data.Select(p => _mapper.Map<PharmacistBO>(p)).ToList();
            pg.Data.ToList().ForEach(p => p.LoginName = p.Email);
            pg.Data.ToList().ForEach(p => p.ApplicationCode = ApplicationNames.PharmacistApp);
            GetUMData(_webApiConf.UserManagementServiceBaseUrl, RoleCodes.Pharmacist, pg, qOrderby,
                qLimit, limit, offset, isActiveFilter, isLockedFilter);
            _logger.LogInformation("GetPharmacists: completed ");

            return pg;
        }
        PagedResults<PharmacistBO> IPharmacistService.GetAllPharamacists()
        {
            PagedResults<PharmacistBO> pg = new PagedResults<PharmacistBO>();
            bool? isActiveFilter = null;
            bool? isLockedFilter = null;
            IFilter filter = GetUsersDefaultFilter(string.Empty, ref isActiveFilter, ref isLockedFilter);

            var pdb = _pharmacistRepository.GetAllPages(0, 0, null, filter);
            pg.TotalCount = pdb.TotalCount;
            pg.PageSize = pdb.PageSize;
            pg.Data = pdb.Data.Select(p => _mapper.Map<PharmacistBO>(p)).ToList();
            pg.Data.ToList().ForEach(p => p.LoginName = p.Email);
            pg.Data.ToList().ForEach(p => p.ApplicationCode = ApplicationNames.PharmacistApp);
            GetUMData(_webApiConf.UserManagementServiceBaseUrl, RoleCodes.Pharmacist, pg, string.Empty,
                0, 0, 0, isActiveFilter, isLockedFilter);
            return pg;
        }
        long IPharmacistService.AddPharmacist(PharmacistBO pharmacist)
        {
            _logger.LogInformation($"AddPharmacist: pharmacist {Newtonsoft.Json.JsonConvert.SerializeObject(pharmacist)}");
            var p = _mapper.Map<Repository.Models.Pharmacist>(pharmacist);
            using (var _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                try
                {
                    using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                    {
                        var res = apiClient.InternalServicePostAsync("api/v1/user/Create", JsonSerializer.Serialize(
                            new BaseUserBO()
                            {
                                LoginName = p.Email,
                                Password = pharmacist.Password,
                                ApplicationCode = ApplicationNames.PharmacistApp,
                                Role = pharmacist.Role,
                                IsActive = pharmacist.IsActive,
                                IsLocked = pharmacist.IsLocked,
                                CreatedBy = pharmacist.CreatedBy
                            })).Result;
                    }
                    p.PhoneVerified = false;
                    _pharmacistRepository.Insert(p);
                    _unitOfWork.Commit();
                    _logger.LogInformation("Pharamist record sucessfully added ");
                }
                catch (AggregateException ex)
                {
                    _unitOfWork.Rollback();
                    throw ex.InnerException;
                }
                catch
                {
                    _unitOfWork.Rollback();
                    throw;
                }
            }
            return p.ID;
        }
        void IPharmacistService.UpdatePharmacist(PharmacistBO pharmacist)
        {
            _logger.LogInformation($"UpdatePharmacist: pharmacist {Newtonsoft.Json.JsonConvert.SerializeObject(pharmacist)}");

            //var p = _mapper.Map<Repository.Models.Pharmacist>(pharmacist);
            using (var _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                try
                {
                    using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                    {
                        var res = apiClient.InternalServicePostAsync("api/v1/user/Update", JsonSerializer.Serialize(
                            new BaseUserBO()
                            {
                                ApplicationCode = ApplicationNames.PharmacistApp,
                                Role = RoleCodes.Pharmacist,
                                LoginName = pharmacist.Email,
                                Password = pharmacist.Password,
                                IsActive = pharmacist.IsActive,
                                IsLocked = pharmacist.IsLocked,
                                UpdatedBy = pharmacist.UpdatedBy,
                            })).Result;
                    }
                    var ph = _pharmacistRepository.GetByID(pharmacist.ID);
                    ph.FirstName = pharmacist.FirstName;
                    ph.LastName = pharmacist.LastName;
                    ph.Street = pharmacist.Street;
                    ph.ZipCode = pharmacist.ZipCode;
                    ph.Address = pharmacist.Address;
                    ph.County = pharmacist.County;
                    if (pharmacist.Phone != ph.Phone)
                    {
                        _logger.LogInformation("Phone number changed so need reverification");
                        ph.PhoneVerified = false;
                        _commonUserServce.UpdateSettings(ph.Email, new List<UserSettingBO>()
                        {
                            new UserSettingBO()
                            {
                                Code = UserSettingCodes.FACTOR_NOTIFICATION_TYPE,
                                DataType = UserSettingDataTypes.LOOKUPS,
                                Value = NotificationType.Email.ToString()
                            }
                        });
                    }
                    ph.Phone = pharmacist.Phone;
                    _pharmacistRepository.Update(ph);
                    _unitOfWork.Commit();
                    _logger.LogInformation("Sucessfully update the pharmacist record");
                }
                catch (AggregateException ex)
                {
                    throw ex.InnerException;
                }
                catch
                {
                    _unitOfWork.Rollback();
                    throw;
                }
            }
        }
        void IPharmacistService.DeletePharmacist(PharmacistBO pharmacist)
        {
            _logger.LogInformation($"DeletePharmacist: pharmacist {Newtonsoft.Json.JsonConvert.SerializeObject(pharmacist)}");

            var p = _mapper.Map<Repository.Models.Pharmacist>(pharmacist);
            _pharmacistRepository.Delete(p);
        }
        PharmacistBO IPharmacistService.GetPharmacistById(long id)
        {
            _logger.LogInformation($"GetPharmacistById: id {id}");

            var pharmacist = _mapper.Map<PharmacistBO>(_pharmacistRepository.GetByID(id));
            if (pharmacist != null)
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync("api/v1/user/loginname/" + pharmacist.Email).Result;
                    var b = JsonSerializer.Deserialize<BaseUserBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    pharmacist.ApplicationCode = ApplicationNames.PharmacistApp;
                    pharmacist.Role = RoleCodes.Pharmacist;
                    pharmacist.LoginName = b.LoginName;
                    pharmacist.IsActive = b.IsActive;
                    pharmacist.IsLocked = b.IsLocked;
                    pharmacist.IsPasswordResetRequired = b.IsPasswordResetRequired;

                }
            }
            else
            {
                _logger.LogWarning("No record found in pharmacist repo");
            }
            return pharmacist;
        }
        PharmacistBO IPharmacistService.GetPharmacistByEmail(string email)
        {
            _logger.LogInformation($"GetPharmacistByEmail: email {email}");
            var pharmacist = _mapper.Map<PharmacistBO>(_pharmacistRepository.GetByEmail(email));
            if (pharmacist != null)
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync("api/v1/user/loginname/" + pharmacist.Email).Result;
                    var b = JsonSerializer.Deserialize<BaseUserBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    pharmacist.ApplicationCode = ApplicationNames.PharmacistApp;
                    pharmacist.Role = RoleCodes.Pharmacist;
                    pharmacist.LoginName = b.LoginName;
                    pharmacist.IsActive = b.IsActive;
                    pharmacist.IsLocked = b.IsLocked;
                    pharmacist.IsPasswordResetRequired = b.IsPasswordResetRequired;
                }
            }
            return pharmacist;
        }
        public string GeneratePhoneVerification(VerifyPhoneRequest req)
        {
            _logger.LogInformation("GeneratePhoneVerification: started");
            SessionContext sessionContext = GetSessionContext();
            var user = _mapper.Map<PharmacistBO>(_pharmacistRepository.GetByEmail(sessionContext.LoginName));
            if (user == null || string.IsNullOrWhiteSpace(req.PhoneNumber))
                throw new ServiceException(ClientSideErrors.INVALID_USER_ACTION);
            var rabbitmq = _serviceProvider.GetRequiredService<RabbitMQClient>();
            int digit = 8;
            var otp = new Random().Next((int)Math.Pow(10, digit - 1) + 1, (int)Math.Pow(10, digit) - 1).ToString("########");
            var sid = DateTime.Now.Ticks.ToString();
            var hash = string.Format("{0}:{1}", sid, otp).Sha256();
            _logger.LogInformation("Generating token for phone verification");
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
        public PharmacistBO VerifyPhoneVerification(VerifyPhoneOtpRequest req)
        {
            try
            {
                _logger.LogInformation("VerifyPhoneVerification: started");
                SessionContext sessionContext = GetSessionContext();
                var user = _pharmacistRepository.GetByEmail(req.Email);
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
                        _logger.LogInformation("Phone Sucessfully verified");
                        user.PhoneVerified = true;
                        user.Phone = req.PhoneNumber;
                        user.UpdatedBy = sessionContext.LoginName;
                        _pharmacistRepository.Update(user);
                        return _mapper.Map<PharmacistBO>(user);
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