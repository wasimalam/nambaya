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
using Pharmacist.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace Pharmacist.Service
{
    public class PharmacyService : BaseUserService, IPharmacyService
    {
        private readonly IPharmacyRepository _pharmacyRepository;
        private readonly IPharmacistRepository _pharmacistRepository;
        private readonly IMapper _mapper;
        private readonly WebServiceConfiguration _webApiConf;
        private readonly ICommonUserServce _commonUserServce;
        private readonly ILogger<PharmacyService> _logger;
        public PharmacyService(IServiceProvider serviceProvider) : base(serviceProvider, false)
        {
            _webApiConf = serviceProvider.GetRequiredService<WebServiceConfiguration>();
            _mapper = serviceProvider.GetRequiredService<IMapper>();
            _pharmacyRepository = serviceProvider.GetRequiredService<IPharmacyRepository>();
            _pharmacistRepository = serviceProvider.GetRequiredService<IPharmacistRepository>();
            _commonUserServce = serviceProvider.GetRequiredService<ICommonUserServce>();
            _logger = serviceProvider.GetRequiredService<ILogger<PharmacyService>>();
        }
        PagedResults<PharmacyBO> IPharmacyService.GetPharmacies(int qLimit, int offset, string qOrderby, string param)
        {
            _logger.LogInformation($"GetPharmacies: qLimit {qLimit} offset {offset} qOrderby {qOrderby} param {param}");
            SessionContext sessionContext = GetSessionContext();
            if (sessionContext.RoleCode != RoleCodes.NambayaUser && sessionContext.RoleCode != RoleCodes.PharmacyTrainer
                && sessionContext.RoleCode != RoleCodes.CentralGroupUser)
                throw new ServiceException(ClientSideErrors.USER_NOT_AUTHORIZED);
            PagedResults<PharmacyBO> pg = new PagedResults<PharmacyBO>();
            bool? isActiveFilter = null;
            bool? isLockedFilter = null;
            IFilter filter = GetUsersDefaultFilter(param, ref isActiveFilter, ref isLockedFilter);
            _logger.LogInformation($"GetPharmacies: user default filter {Newtonsoft.Json.JsonConvert.SerializeObject(filter)}");

            var orderby = getAdjustedOrderBy(qOrderby, isActiveFilter, isLockedFilter);
            int limit = getAdjustedQueryLimit(qOrderby, qLimit, isActiveFilter, isLockedFilter);
            var pdb = _pharmacyRepository.GetAllPages(limit, offset, orderby, filter);
            pg.TotalCount = pdb.TotalCount;
            pg.PageSize = pdb.PageSize;
            pg.Data = pdb.Data.Select(p => _mapper.Map<PharmacyBO>(p)).ToList();
            pg.Data.ToList().ForEach(p => p.LoginName = p.Email);
            pg.Data.ToList().ForEach(p => p.ApplicationCode = ApplicationNames.PharmacistApp);
            GetUMData(_webApiConf.UserManagementServiceBaseUrl, RoleCodes.Pharmacy, pg, qOrderby,
                qLimit, limit, offset, isActiveFilter, isLockedFilter);
            _logger.LogInformation($"GetPharmacies: Sucessfully fetched pharmacies data..");
            return pg;
        }
        PagedResults<PharmacyBO> IPharmacyService.GetAllPharamacists()
        {
            PagedResults<PharmacyBO> pg = new PagedResults<PharmacyBO>();
            bool? isActiveFilter = null;
            bool? isLockedFilter = null;
            IFilter filter = GetUsersDefaultFilter(string.Empty, ref isActiveFilter, ref isLockedFilter);

            var pdb = _pharmacyRepository.GetAllPages(0, 0, null, filter);
            pg.TotalCount = pdb.TotalCount;
            pg.PageSize = pdb.PageSize;
            pg.Data = pdb.Data.Select(p => _mapper.Map<PharmacyBO>(p)).ToList();
            pg.Data.ToList().ForEach(p => p.LoginName = p.Email);
            pg.Data.ToList().ForEach(p => p.ApplicationCode = ApplicationNames.PharmacistApp);
            GetUMData(_webApiConf.UserManagementServiceBaseUrl, RoleCodes.Pharmacy, pg, string.Empty,
                0, 0, 0, isActiveFilter, isLockedFilter);
            return pg;
        }
        public string GetPharmacyIdentification()
        {
            _logger.LogInformation($"GetPharmacyIdentification: Started");

            SessionContext sessionContext = GetSessionContext();
            long pharmacyid = sessionContext.AppUserID;
            if (sessionContext.RoleCode == RoleCodes.Pharmacist)
            {
                _logger.LogInformation($"Finding pharmacy if the role code is {RoleCodes.Pharmacist}");
                pharmacyid = _pharmacistRepository.GetByID(sessionContext.AppUserID).PharmacyID;
                _logger.LogInformation($"Pharmacy id is {pharmacyid}");
            }
            else if (sessionContext.RoleCode != RoleCodes.Pharmacy)
                throw new ServiceException(ClientSideErrors.INVALID_USER_ACTION);
            return _pharmacyRepository.GetByID(pharmacyid).Identification;
        }
        long IPharmacyService.AddPharmacy(PharmacyBO pharmacy)
        {
            var p = _mapper.Map<Pharmacy>(pharmacy);
            _logger.LogInformation($"AddPharmacy: pharmacy object {Newtonsoft.Json.JsonConvert.SerializeObject(pharmacy)}");
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
                                Password = pharmacy.Password,
                                ApplicationCode = ApplicationNames.PharmacistApp,
                                Role = pharmacy.Role,
                                IsActive = pharmacy.IsActive,
                                IsLocked = pharmacy.IsLocked,
                                CreatedBy = pharmacy.CreatedBy
                            })).Result;
                    }
                    p.PhoneVerified = false;
                    _pharmacyRepository.Insert(p);
                    _logger.LogInformation("Sucesfully added the pharamcy in database");
                    _unitOfWork.Commit();
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
        void IPharmacyService.UpdatePharmacy(PharmacyBO pharmacy)
        {
            var p = _mapper.Map<Pharmacy>(pharmacy);
            _logger.LogInformation($"UpdatePharmacy: pharmacy object {Newtonsoft.Json.JsonConvert.SerializeObject(pharmacy)}");
            
            var dbUser = _pharmacyRepository.GetByID(pharmacy.ID);
            using (var _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                try
                {
                    using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                    {
                        _logger.LogInformation("Calling api for updating pharmacy ");
                        var res = apiClient.InternalServicePostAsync("api/v1/user/Update", JsonSerializer.Serialize(
                            new BaseUserBO()
                            {
                                ApplicationCode = ApplicationNames.PharmacistApp,
                                Role = RoleCodes.Pharmacy,
                                LoginName = p.Email,
                                Password = pharmacy.Password,
                                IsActive = pharmacy.IsActive,
                                IsLocked = pharmacy.IsLocked,
                                UpdatedBy = pharmacy.UpdatedBy,
                            })).Result;
                    }
                    p.PhoneVerified = dbUser.PhoneVerified;
                    if (p.Phone != dbUser.Phone)
                    {
                        _logger.LogInformation("Phone number changed need reverification");
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
                    _pharmacyRepository.Update(p);
                    _logger.LogInformation("Pharmacy record sucessfully updated");
                    _unitOfWork.Commit();
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
        void IPharmacyService.DeletePharmacy(PharmacyBO pharmacy)
        {
            _logger.LogInformation($"DeletePharmacy: pharmacy {Newtonsoft.Json.JsonConvert.SerializeObject(pharmacy)}");
            var p = _mapper.Map<Pharmacy>(pharmacy);
            _pharmacyRepository.Delete(p);
        }
        long IPharmacyService.GetTotal()
        {
            return _pharmacyRepository.GetCount();
        }
        PharmacyBO IPharmacyService.GetPharmacyById(long id)
        {
            _logger.LogInformation($"Getting pharmacy by id {id}");
            var pharma = _mapper.Map<PharmacyBO>(_pharmacyRepository.GetByID(id));
            if (pharma != null)
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync("api/v1/user/loginname/" + pharma.Email).Result;
                    var b = JsonSerializer.Deserialize<BaseUserBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    pharma.ApplicationCode = ApplicationNames.PharmacistApp;
                    pharma.Role = RoleCodes.Pharmacy;
                    pharma.LoginName = b.LoginName;
                    pharma.IsActive = b.IsActive;
                    pharma.IsLocked = b.IsLocked;
                    pharma.IsPasswordResetRequired = b.IsPasswordResetRequired;
                }
            }
            else
            {
                _logger.LogWarning("No record found in pharmacy respositor");
            }
            return pharma;
        }
        PharmacyBO IPharmacyService.GetPharmacyByEmail(string email)
        {
            _logger.LogInformation($"GetPharmacyByEmail: email {email}");

            var pharma = _mapper.Map<PharmacyBO>(_pharmacyRepository.GetByEmail(email));
            if (pharma != null)
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync("api/v1/user/loginname/" + pharma.Email).Result;
                    var b = JsonSerializer.Deserialize<BaseUserBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    pharma.ApplicationCode = ApplicationNames.PharmacistApp;
                    pharma.Role = RoleCodes.Pharmacy;
                    pharma.LoginName = b.LoginName;
                    pharma.IsActive = b.IsActive;
                    pharma.IsLocked = b.IsLocked;
                    pharma.IsPasswordResetRequired = b.IsPasswordResetRequired;
                }
            }
            else
            {
                _logger.LogWarning("No record found in pharmacy respository");
            }
            return pharma;
        }
        public PharmacyBO GetPharmacyByPharmacist(string pharmacistEmail)
        {
            _logger.LogInformation($"GetPharmacyByPharmacist: pharmacistEmail {pharmacistEmail}");

            var pharmacy = _mapper.Map<PharmacyBO>(_pharmacyRepository.GetByPharmacist(pharmacistEmail));
            if (pharmacy != null)
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync("api/v1/user/loginname/" + pharmacy.Email).Result;
                    var b = JsonSerializer.Deserialize<BaseUserBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    pharmacy.ApplicationCode = ApplicationNames.PharmacistApp;
                    pharmacy.Role = RoleCodes.Pharmacy;
                    pharmacy.LoginName = b.LoginName;
                    pharmacy.IsActive = b.IsActive;
                    pharmacy.IsLocked = b.IsLocked;
                    pharmacy.IsPasswordResetRequired = b.IsPasswordResetRequired;
                }
            }
            else
            {
                _logger.LogWarning("No record found in pharmacy respository");
            }
            return pharmacy;
        }
        public string GeneratePhoneVerification(VerifyPhoneRequest req)
        {
            _logger.LogInformation("GeneratePhoneVerification: starting verification");
            SessionContext sessionContext = GetSessionContext();
            var user = _mapper.Map<PharmacyBO>(_pharmacyRepository.GetByEmail(sessionContext.LoginName));
            if (user == null || string.IsNullOrWhiteSpace(req.PhoneNumber))
                throw new ServiceException(ClientSideErrors.INVALID_USER_ACTION);
            var rabbitmq = _serviceProvider.GetRequiredService<RabbitMQClient>();
            int digit = 8;
            var otp = new Random().Next((int)Math.Pow(10, digit - 1) + 1, (int)Math.Pow(10, digit) - 1).ToString("########");
            var sid = DateTime.Now.Ticks.ToString();
            var hash = string.Format("{0}:{1}", sid, otp).Sha256();
            _logger.LogInformation("Generating token for phone verfication ");
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
        public PharmacyBO VerifyPhoneVerification(VerifyPhoneOtpRequest req)
        {
            _logger.LogInformation("VerifyPhoneVerification: started ");
            try
            {
                SessionContext sessionContext = GetSessionContext();
                var user = _pharmacyRepository.GetByEmail(req.Email);
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
                        _pharmacyRepository.Update(user);
                        return _mapper.Map<PharmacyBO>(user);
                    }
                }
                else
                {
                    _logger.LogInformation("VerifyPhoneVerification: User identity is not authunticated");
                }
            }
            catch
            {
            }
            throw new ServiceException(ClientSideErrors.OTP_VERIFICATION_FAILED);
        }
    }
}