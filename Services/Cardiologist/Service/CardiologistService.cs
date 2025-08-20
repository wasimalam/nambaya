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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace Cardiologist.Service
{
    public class CardiologistService : BaseUserService, ICardiologistService
    {
        private readonly ICardiologistRepository _cardiologistRepository;
        private readonly IMapper _mapper;
        private readonly WebServiceConfiguration _webApiConf;
        private readonly ICommonUserServce _commonUserServce;
        private readonly RabbitMQClient _rabbitMqClient;
        private readonly ILogger<CardiologistService> _logger;
        public CardiologistService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _webApiConf = serviceProvider.GetRequiredService<WebServiceConfiguration>();
            _mapper = serviceProvider.GetRequiredService<IMapper>();
            _cardiologistRepository = serviceProvider.GetRequiredService<ICardiologistRepository>();
            _commonUserServce = serviceProvider.GetRequiredService<ICommonUserServce>();
            _rabbitMqClient = serviceProvider.GetRequiredService<RabbitMQClient>();
            _logger = serviceProvider.GetRequiredService<ILogger<CardiologistService>>();
        }
        PagedResults<CardiologistBO> ICardiologistService.GetCardiologists(int qLimit, int offset, string qOrderby, string param)
        {
            _logger.LogInformation($"GetCardiologists: Filter limit : {qLimit}, offset {offset}, orderby: {qOrderby}, param {param} ");

            PagedResults<CardiologistBO> pg = new PagedResults<CardiologistBO>();
            bool? isActiveFilter = null;
            bool? isLockedFilter = null;
            IFilter filter = GetUsersDefaultFilter(param, ref isActiveFilter, ref isLockedFilter);
            var orderby = getAdjustedOrderBy(qOrderby, isActiveFilter, isLockedFilter);
            int limit = getAdjustedQueryLimit(qOrderby, qLimit, isActiveFilter, isLockedFilter);
            var pdb = _cardiologistRepository.GetAllPages(limit, offset, orderby, filter);
            pg.TotalCount = pdb.TotalCount;
            pg.PageSize = pdb.PageSize;
            pg.Data = pdb.Data.Select(p => _mapper.Map<CardiologistBO>(p)).ToList();
            pg.Data.ToList().ForEach(p => p.LoginName = p.Email);
            pg.Data.ToList().ForEach(p => p.ApplicationCode = ApplicationNames.CardiologistApp);
            GetUMData(_webApiConf.UserManagementServiceBaseUrl, RoleCodes.Cardiologist, pg, qOrderby,
                qLimit, limit, offset, isActiveFilter, isLockedFilter);
            _logger.LogInformation($"GetCardiologists: Sucess ");

            return pg;
        }
        CardiologistBO ICardiologistService.GetCardiologistById(long id)
        {
            _logger.LogInformation($"GetCardiologistById: id {id} ");

            var card = _mapper.Map<CardiologistBO>(_cardiologistRepository.GetByID(id));
            if (card != null)
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync("api/v1/user/loginname/" + card.Email).Result;
                    var b = JsonSerializer.Deserialize<BaseUserBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    card.ApplicationCode = ApplicationNames.CardiologistApp;
                    card.Role = RoleCodes.Cardiologist;
                    card.LoginName = b.LoginName;
                    card.IsActive = b.IsActive;
                    card.IsLocked = b.IsLocked;
                }
            }
            else
            {
                _logger.LogWarning($"No Cardiologist card found against id {id} ");
            }
            return card;
        }
        CardiologistBO ICardiologistService.GetCardiologistByEmail(string email)
        {
            _logger.LogInformation($"GetCardiologistByEmail: email  {email} ");

            var card = _mapper.Map<CardiologistBO>(_cardiologistRepository.GetByEmail(email));
            if (card != null)
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync("api/v1/user/loginname/" + card.Email).Result;
                    var b = JsonSerializer.Deserialize<BaseUserBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    card.ApplicationCode = ApplicationNames.CardiologistApp;
                    card.Role = RoleCodes.Cardiologist;
                    card.LoginName = b.LoginName;
                    card.IsActive = b.IsActive;
                    card.IsLocked = b.IsLocked;
                    card.IsPasswordResetRequired = b.IsPasswordResetRequired;
                }
            }
            else
            {
                _logger.LogWarning($"No Cardiologist card found against email  {email} ");
            }
            return card;
        }
        long ICardiologistService.GetTotal()
        {
            return _cardiologistRepository.GetCount();
        }
        long ICardiologistService.AddCardiologist(CardiologistBO cardiologist)
        {
            _logger.LogInformation($"Add new Cardiologist {Newtonsoft.Json.JsonConvert.SerializeObject(cardiologist)}");

            var p = _mapper.Map<Repository.Models.Cardiologist>(cardiologist);
            using (var _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                try
                {
                    p.PhoneVerified = false;
                    _cardiologistRepository.Insert(p);
                    using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                    {
                        var res = apiClient.InternalServicePostAsync("api/v1/user/Create", JsonSerializer.Serialize(
                            new BaseUserBO()
                            {
                                ApplicationCode = ApplicationNames.CardiologistApp,
                                LoginName = cardiologist.Email,
                                Role = cardiologist.Role,
                                Password = cardiologist.Password,
                                IsActive = cardiologist.IsActive,
                                IsLocked = cardiologist.IsLocked,
                                CreatedBy = cardiologist.CreatedBy
                            })).Result;
                    }
                    _unitOfWork.Commit();
                    _logger.LogInformation($"Cardiologist Information sucessfully updated");
                    //_rabbitMqClient.SendMessage(KnownChannels.NAVIGATOR_CREATE_CARDIOLOGIST, _mapper.Map<CardiologistBO>(p));
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

        void ICardiologistService.UpdateCardiologist(CardiologistBO cardiologist)
        {
            _logger.LogInformation($"Start to update Cardiologist {Newtonsoft.Json.JsonConvert.SerializeObject(cardiologist)}");

            var p = _mapper.Map<Repository.Models.Cardiologist>(cardiologist);
            var dbUser = _cardiologistRepository.GetByID(cardiologist.ID);
            using (var _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                try
                {
                    using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                    {
                        var res = apiClient.InternalServicePostAsync("api/v1/user/Update", JsonSerializer.Serialize(
                            new BaseUserBO()
                            {
                                ApplicationCode = ApplicationNames.CardiologistApp,
                                Role = RoleCodes.Cardiologist,
                                LoginName = cardiologist.Email,
                                Password = cardiologist.Password,
                                IsActive = cardiologist.IsActive,
                                IsLocked = cardiologist.IsLocked,
                                UpdatedBy = cardiologist.UpdatedBy,
                            })).Result;
                    }
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
                    _cardiologistRepository.Update(p);
                    _unitOfWork.Commit();
                    _logger.LogInformation($"Cardiologist Information sucessfully updated");
                    //_rabbitMqClient.SendMessage(KnownChannels.NAVIGATOR_UPDATE_CARDIOLOGIST, _mapper.Map<CardiologistBO>(p));
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

        void ICardiologistService.DeleteCardiologist(CardiologistBO cardiologist)
        {
            var p = _mapper.Map<Repository.Models.Cardiologist>(cardiologist);
            _cardiologistRepository.Delete(p);
        }
        public string GeneratePhoneVerification(VerifyPhoneRequest req)
        {
            SessionContext sessionContext = GetSessionContext();
            var user = _mapper.Map<CardiologistBO>(_cardiologistRepository.GetByEmail(sessionContext.LoginName));
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
        public CardiologistBO VerifyPhoneVerification(VerifyPhoneOtpRequest req)
        {
            try
            {
                SessionContext sessionContext = GetSessionContext();
                var user = _cardiologistRepository.GetByEmail(req.Email);
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
                        _logger.LogInformation("Phone verified sucessfully ");
                        user.PhoneVerified = true;
                        user.Phone = req.PhoneNumber;
                        user.UpdatedBy = sessionContext.LoginName;
                        _cardiologistRepository.Update(user);
                        return _mapper.Map<CardiologistBO>(user);
                    }
                }
                else
                {
                    _logger.LogWarning($"User identity is not autheticated");
                }
            }
            catch
            {
            }
            throw new ServiceException(ClientSideErrors.OTP_VERIFICATION_FAILED);
        }
    }
}
