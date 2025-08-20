using AutoMapper;
using Cardiologist.Contracts.Interfaces;
using Cardiologist.Contracts.Models;
using Cardiologist.Repository.Interfaces;
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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace Nurse.Service
{
    public class NurseService : BaseUserService, INurseService
    {
        private readonly INurseRepository _nurseRepository;
        private readonly IMapper _mapper;
        private readonly WebServiceConfiguration _webApiConf;
        private readonly ICommonUserServce _commonUserServce;
        private readonly ILogger<NurseService> _logger;
        private readonly RabbitMQClient _rabbitMqClient;
        public NurseService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _webApiConf = serviceProvider.GetRequiredService<WebServiceConfiguration>();
            _mapper = serviceProvider.GetRequiredService<IMapper>();
            _nurseRepository = serviceProvider.GetRequiredService<INurseRepository>();
            _commonUserServce = serviceProvider.GetRequiredService<ICommonUserServce>();
            _rabbitMqClient = serviceProvider.GetRequiredService<RabbitMQClient>();
            _logger = serviceProvider.GetRequiredService<ILogger<NurseService>>();
        }
        PagedResults<NurseBO> INurseService.GetNurses(int qLimit, int offset, string qOrderby, string param)
        {
            var sessionContext = GetSessionContext();
            if (sessionContext.RoleCode != RoleCodes.Cardiologist)
                throw new ServiceException(ClientSideErrors.USER_NOT_AUTHORIZED);
            PagedResults<NurseBO> pg = new PagedResults<NurseBO>();
            bool? isActiveFilter = null;
            bool? isLockedFilter = null;
            _logger.LogInformation($"GetNurses: Filter qLimit : {qLimit}, offset {offset}, qOrderby: {qOrderby}, param{param} ");
            IFilter filter = GetUsersDefaultFilter(param, ref isActiveFilter, ref isLockedFilter);
            _logger.LogInformation($"GetNurses: Filter {filter}");
            IFilter cardiologistFilter = new ANDFilter(new List<IFilter>() { new Filter(strFilterName: "cardiologistid", sqlOperator: SqlOperators.Equal, strFilterValue: sessionContext.CardiologistID) });
            if (filter == null)
                filter = cardiologistFilter;
            else
                filter = new ANDFilter(filter, cardiologistFilter);
            var orderby = getAdjustedOrderBy(qOrderby, isActiveFilter, isLockedFilter);
            int limit = getAdjustedQueryLimit(qOrderby, qLimit, isActiveFilter, isLockedFilter);
            var pdb = _nurseRepository.GetAllPages(limit, offset, orderby, filter);
            pg.TotalCount = pdb.TotalCount;
            pg.PageSize = pdb.PageSize;
            pg.Data = pdb.Data.Select(p => _mapper.Map<NurseBO>(p)).ToList();
            pg.Data.ToList().ForEach(p => p.LoginName = p.Email);
            pg.Data.ToList().ForEach(p => p.ApplicationCode = ApplicationNames.CardiologistApp);
            GetUMData(_webApiConf.UserManagementServiceBaseUrl, RoleCodes.Nurse, pg, qOrderby,
                qLimit, limit, offset, isActiveFilter, isLockedFilter);
            _logger.LogInformation($"GetNurses: Sucessfully filtered the Nurses Data");
            return pg;
        }
        PagedResults<NurseBO> INurseService.GetAllNurses(int qLimit, int offset, string qOrderby, string param)
        {
            _logger.LogInformation($"GetAllNurses: Filter qLimit : {qLimit}, offset {offset}, qOrderby: {qOrderby}, param {param} ");
            PagedResults <NurseBO> pg = new PagedResults<NurseBO>();
            bool? isActiveFilter = null;
            bool? isLockedFilter = null;
            IFilter filter = GetUsersDefaultFilter(param, ref isActiveFilter, ref isLockedFilter);
            
            var orderby = getAdjustedOrderBy(qOrderby, isActiveFilter, isLockedFilter);
            int limit = getAdjustedQueryLimit(qOrderby, qLimit, isActiveFilter, isLockedFilter);
            var pdb = _nurseRepository.GetAllPages(limit, offset, orderby, filter);
            pg.TotalCount = pdb.TotalCount;
            pg.PageSize = pdb.PageSize;
            pg.Data = pdb.Data.Select(p => _mapper.Map<NurseBO>(p)).ToList();
            pg.Data.ToList().ForEach(p => p.LoginName = p.Email);
            pg.Data.ToList().ForEach(p => p.ApplicationCode = ApplicationNames.CardiologistApp);
            GetUMData(_webApiConf.UserManagementServiceBaseUrl, RoleCodes.Nurse, pg, qOrderby,
                qLimit, limit, offset, isActiveFilter, isLockedFilter);
            _logger.LogInformation($"GetAllNurses: Sucessfully filtered the Nurses Data");
            return pg;
        }
        NurseBO INurseService.GetNurseById(long id)
        {
            _logger.LogInformation($"GetNurseById: find nurse against id: {id} ");
            var card = _mapper.Map<NurseBO>(_nurseRepository.GetByID(id));
            if (card != null)
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync("api/v1/user/loginname/" + card.Email).Result;
                    var b = System.Text.Json.JsonSerializer.Deserialize<BaseUserBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    card.ApplicationCode = ApplicationNames.CardiologistApp;
                    card.Role = RoleCodes.Nurse;
                    card.LoginName = b.LoginName;
                    card.IsActive = b.IsActive;
                    card.IsLocked = b.IsLocked;
                }
            }

            return card;
        }
        NurseBO INurseService.GetNurseByEmail(string email)
        {
            _logger.LogInformation($"GetNurseByEmail: find nurse against email: {email} ");

            var card = _mapper.Map<NurseBO>(_nurseRepository.GetByEmail(email));
            if (card != null)
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync("api/v1/user/loginname/" + card.Email).Result;
                    var b = System.Text.Json.JsonSerializer.Deserialize<BaseUserBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    card.ApplicationCode = ApplicationNames.CardiologistApp;
                    card.Role = RoleCodes.Nurse;
                    card.LoginName = b.LoginName;
                    card.IsActive = b.IsActive;
                    card.IsLocked = b.IsLocked;
                    card.IsPasswordResetRequired = b.IsPasswordResetRequired;
                }
            }
            return card;
        }
        long INurseService.GetTotal()
        {
            return _nurseRepository.GetCount();
        }
        long INurseService.AddNurse(NurseBO nurse)
        {
            var sessionContext = GetSessionContext();
            _logger.LogInformation($"Start to Add New Nurse {JsonConvert.SerializeObject(nurse)}");
            var p = _mapper.Map<Cardiologist.Repository.Models.Nurse>(nurse);
            using (var _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                try
                {
                    p.PhoneVerified = false;
                    p.CardiologistID = sessionContext.CardiologistID;
                    _nurseRepository.Insert(p);
                    _logger.LogInformation($"Nurse Added in database");
                    using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                    {
                        var res = apiClient.InternalServicePostAsync("api/v1/user/Create", System.Text.Json.JsonSerializer.Serialize(
                            new BaseUserBO()
                            {
                                ApplicationCode = ApplicationNames.CardiologistApp,
                                LoginName = nurse.Email,
                                Role = nurse.Role,
                                Password = nurse.Password,
                                IsActive = nurse.IsActive,
                                IsLocked = nurse.IsLocked,
                                CreatedBy = nurse.CreatedBy
                            })).Result;
                    }
                    _unitOfWork.Commit();
                    //_rabbitMqClient.SendMessage(KnownChannels.NAVIGATOR_CREATE_CARDIOLOGIST, _mapper.Map<NurseBO>(p));
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
        void INurseService.UpdateNurse(NurseBO nurse)
        {
            _logger.LogInformation($"Start to update Nurse {JsonConvert.SerializeObject(nurse)}");
            var sessionContext = GetSessionContext();
            var p = _mapper.Map<Cardiologist.Repository.Models.Nurse>(nurse);
            
            var dbUser = _nurseRepository.GetByID(nurse.ID);
            _logger.LogInformation($"Current Nurse user in database is {JsonConvert.SerializeObject(nurse)}");
            if (dbUser.CardiologistID != sessionContext.CardiologistID)
                throw new ServiceException(ClientSideErrors.INVALID_USER_ACTION);
            using (var _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                try
                {
                    using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                    {
                        var res = apiClient.InternalServicePostAsync("api/v1/user/Update", System.Text.Json.JsonSerializer.Serialize(
                            new BaseUserBO()
                            {
                                ApplicationCode = ApplicationNames.CardiologistApp,
                                Role = RoleCodes.Nurse,
                                LoginName = nurse.Email,
                                Password = nurse.Password,
                                IsActive = nurse.IsActive,
                                IsLocked = nurse.IsLocked,
                                UpdatedBy = nurse.UpdatedBy,
                            })).Result;
                        _logger.LogInformation($"Response get from apiClient.InternalServicePostAsync {res}");

                    }
                    p.CardiologistID = sessionContext.CardiologistID;
                    p.PhoneVerified = dbUser.PhoneVerified;
                    if (p.Phone != dbUser.Phone)
                    {
                        p.PhoneVerified = false;
                        _commonUserServce.UpdateSettings(p.Email, new List<UserSettingBO>(){
                            new UserSettingBO()
                            {
                                Code = UserSettingCodes.FACTOR_NOTIFICATION_TYPE,
                                DataType = UserSettingDataTypes.LOOKUPS,
                                Value = NotificationType.Email.ToString()
                            }
                        });
                    }
                    _nurseRepository.Update(p);
                    _unitOfWork.Commit();
                    _logger.LogInformation($"Nurse Information sucessfully updated");
                    //_rabbitMqClient.SendMessage(KnownChannels.NAVIGATOR_UPDATE_CARDIOLOGIST, _mapper.Map<NurseBO>(p));
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

        void INurseService.DeleteNurse(NurseBO nurse)
        {
            var sessionContext = GetSessionContext();
            if (nurse.CardiologistID != sessionContext.CardiologistID)
                throw new ServiceException(ClientSideErrors.INVALID_USER_ACTION);
            var p = _mapper.Map<Cardiologist.Repository.Models.Nurse>(nurse);
            _nurseRepository.Delete(p);
            _logger.LogInformation($"Nurse Information sucessfully Deleted");

        }
        public string GeneratePhoneVerification(VerifyPhoneRequest req)
        {
            SessionContext sessionContext = GetSessionContext();
            var user = _mapper.Map<NurseBO>(_nurseRepository.GetByEmail(sessionContext.LoginName));
            if (string.IsNullOrWhiteSpace(req.PhoneNumber))
            {
                _logger.LogWarning($"Phone number is null for phone verfication");
                throw new ServiceException(ClientSideErrors.INVALID_USER_ACTION);
            }
                
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
        public NurseBO VerifyPhoneVerification(VerifyPhoneOtpRequest req)
        {
            try
            {
                SessionContext sessionContext = GetSessionContext();
                var user = _nurseRepository.GetByEmail(req.Email);
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
                        _nurseRepository.Update(user);
                        return _mapper.Map<NurseBO>(user);
                    }
                }
                else
                {
                    _logger.LogWarning($"VerifyPhoneVerification: User is not autheticated for phone verfication ");
                }
            }
            catch
            {
            }
            throw new ServiceException(ClientSideErrors.OTP_VERIFICATION_FAILED);
        }
    }
}
