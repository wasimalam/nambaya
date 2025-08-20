using AutoMapper;
using Cardiologist.Contracts.Interfaces;
using Cardiologist.Contracts.Models;
using Cardiologist.Repository.Interfaces;
using Common.BusinessObjects;
using Common.BusinessObjects.Interfaces;
using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Common.Infrastructure.Extensions;
using Common.Infrastructure.Helpers;
using Common.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;

namespace Cardiologist.Service
{
    public class SignatureService : BaseUserService, ISignatureService
    {
        private readonly ICardiologistRepository _cardiologistRepository;
        private readonly ISignaturesRepository _signaturesRepository;
        private readonly IMapper _mapper;
        private readonly WebServiceConfiguration _webApiConf;
        private readonly ICommonUserServce _commonUserServce;
        private readonly ILogger<SignatureService> _logger;
        public SignatureService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _webApiConf = serviceProvider.GetRequiredService<WebServiceConfiguration>();
            _mapper = serviceProvider.GetRequiredService<IMapper>();
            _cardiologistRepository = serviceProvider.GetRequiredService<ICardiologistRepository>();
            _signaturesRepository = serviceProvider.GetRequiredService<ISignaturesRepository>();
            _commonUserServce = serviceProvider.GetRequiredService<ICommonUserServce>();
            _logger = serviceProvider.GetRequiredService<ILogger<SignatureService>>();
        }

        public SignaturesBO GetSignatures()
        {
            _logger.LogTrace($"Entered GetSignatures");
            SessionContext sessionContext = GetSessionContext();
            if (sessionContext.RoleCode == RoleCodes.Cardiologist)
            {
                _logger.LogInformation($"Getting signature for Cardiologist");
                var sig = _mapper.Map<SignaturesBO>(_signaturesRepository.GetByCardiologist(sessionContext.AppUserID));
                if (sig != null)
                {
                    using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.FileSharingServiceBaseUrl, TimeSpan.FromMinutes(15)))
                    {
                        var data = apiClient.DownloadGetAsync($"api/v1/filesharing/{sig.FilePath}", "application/x-www-form-urlencoded", "*/*").Result;
                        sig.FileDataString = Convert.ToBase64String(data.Take((int)sig.FileLength).ToArray());
                    }
                }
                return sig;
            }
            throw new ServiceException(ClientSideErrors.INVALID_USER_ACTION);
        }

        public string GenerateSignatureSaveOTP()
        {
            _logger.LogTrace($"Entered GenerateSignatureSaveOTP");

            SessionContext sessionContext = GetSessionContext();
            var user = _mapper.Map<CardiologistBO>(_cardiologistRepository.GetByEmail(sessionContext.LoginName));
            var rabbitmq = _serviceProvider.GetRequiredService<RabbitMQClient>();
            int digit = 8;
            var otp = new Random().Next((int)Math.Pow(10, digit - 1) + 1, (int)Math.Pow(10, digit) - 1).ToString("########");
            var sid = DateTime.Now.Ticks.ToString();
            var hash = string.Format("{0}:{1}", sid, otp).Sha256();
            var token = JwtWrapper.GenerateToken(ConfigurationConsts.UpdateSignatureVerificationKey.Sha256(),
                new Claim[]
                {
                  new Claim("sid", sid),
                  new Claim("otp_email", user.Email),
                  new Claim("otp_name", user.Name??""),
                  new Claim("otp_hash", hash),
                }, 120);
            rabbitmq.SendMessage(KnownChannels.SIGNATURE_SAVE_EVENT_CHANNEL, new UserOtp()
            {
                AppUserID = user.ID,
                Email = user.Email,
                Name = user.Name,
                Phone = user.Phone,
                OTP = otp,
                SMS = user.PhoneVerified && _commonUserServce.GetSettings(user.Email).FirstOrDefault(p => p.Code == UserSettingCodes.FACTOR_NOTIFICATION_TYPE)?.Value == NotificationType.SMS.ToString(),
            });
            return token;
        }
        public SignaturesBO VerifySignatureSave(UpdateSignatureOtpRequest req)
        {
            try
            {
                _logger.LogTrace($"Entered VerifySignatureSave");

                SessionContext sessionContext = GetSessionContext();
                var user = _cardiologistRepository.GetByEmail(req.Email);
                var principle = JwtWrapper.GetClaimsPrincipal(ConfigurationConsts.UpdateSignatureVerificationKey.Sha256(), req.Token);
                if (principle?.Identity?.IsAuthenticated == true)
                {
                    _logger.LogInformation($"VerifySignatureSave: user identity is authenticated");
                    var claimsIdentity = principle.Identity as ClaimsIdentity;
                    var claims = claimsIdentity.Claims.ToArray();
                    var sid = claims.Single(c => c.Type == "sid").Value;
                    var hash = claims.Single(c => c.Type == "otp_hash").Value;
                    if (hash == string.Format("{0}:{1}", sid, req.OTP.ToString()).Sha256() &&
                        claims.Single(c => c.Type == "otp_email").Value == req.Email)
                    {
                        using (var _unitOfWork = _serviceProvider.GetRequiredService<Common.DataAccess.Interfaces.IUnitOfWork>())
                        {
                            try
                            {
                                var sig = _signaturesRepository.GetByCardiologist(sessionContext.AppUserID);
                                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.FileSharingServiceBaseUrl, TimeSpan.FromMinutes(15)))
                                {
                                    var data = Convert.FromBase64String(req.ImageData);
                                    var filename = Guid.NewGuid().ToString();
                                    if (sig == null)
                                    {
                                        sig = new Repository.Models.Signatures()
                                        {
                                            CardiologistID = sessionContext.AppUserID,
                                            FilePath = $"cardio/{sessionContext.AppUserID}/{filename}",
                                            FileLength = data.Length,
                                            CreatedBy = sessionContext.LoginName
                                        };
                                        _signaturesRepository.Insert(sig);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            _logger.LogInformation($"Delete the previous cardiologist signature");
                                            var res1 = apiClient.DeleteInternalAsync($"api/v1/filesharing/{sig.FilePath}").Result;
                                        }
                                        catch (Exception se)
                                        {
                                            if (se.InnerException != null && se.InnerException is ServiceException && se.InnerException.Message == ClientSideErrors.RESOURCE_NOT_FOUND)
                                            {
                                            }
                                            else throw;
                                        }
                                        sig.FilePath = $"cardio/{sessionContext.AppUserID}/{filename}";
                                        sig.FileLength = data.Length;
                                        sig.UpdatedBy = sessionContext.LoginName;
                                        _signaturesRepository.Update(sig);
                                        _logger.LogInformation($"Sucessfully update the  cardiologist signature");
                                    }
                                    var res = apiClient.PostFileAsync($"api/v1/filesharing/{sig.FilePath}", filename, data).Result;
                                    _unitOfWork.Commit();
                                }
                                return _mapper.Map<SignaturesBO>(sig);
                            }
                            catch (Exception ex)
                            {
                                _unitOfWork.Rollback();
                                _logger.LogError(ex, ex.Message);
                                throw;
                            }
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"VerifySignatureSave: OTP or email is not matched");

                    }
                }
                else
                {
                    _logger.LogInformation($"VerifySignatureSave: user identity is not authenticated");
                }
            }
            catch
            {
            }
            throw new ServiceException(ClientSideErrors.OTP_VERIFICATION_FAILED);
        }

        public string GenerateSignatureDeleteOTP()
        {
            _logger.LogInformation($"Entered GenerateSignatureDeleteOTP");
            SessionContext sessionContext = GetSessionContext();
            var user = _mapper.Map<CardiologistBO>(_cardiologistRepository.GetByEmail(sessionContext.LoginName));
            var rabbitmq = _serviceProvider.GetRequiredService<RabbitMQClient>();
            int digit = 8;
            var otp = new Random().Next((int)Math.Pow(10, digit - 1) + 1, (int)Math.Pow(10, digit) - 1).ToString("########");
            var sid = DateTime.Now.Ticks.ToString();
            var hash = string.Format("{0}:{1}", sid, otp).Sha256();
            var token = JwtWrapper.GenerateToken(ConfigurationConsts.DeleteSignatureVerificationKey.Sha256(),
                new Claim[]
                {
                  new Claim("sid", sid),
                  new Claim("otp_email", user.Email),
                  new Claim("otp_name", user.Name??""),
                  new Claim("otp_hash", hash),
                }, 120);
            rabbitmq.SendMessage(KnownChannels.SIGNATURE_DELETE_EVENT_CHANNEL, new UserOtp()
            {
                AppUserID = user.ID,
                Email = user.Email,
                Name = user.Name,
                Phone = user.Phone,
                OTP = otp,
                SMS = user.PhoneVerified && _commonUserServce.GetSettings(user.Email).FirstOrDefault(p => p.Code == UserSettingCodes.FACTOR_NOTIFICATION_TYPE)?.Value == NotificationType.SMS.ToString(),
            });
            return token;
        }
        public void VerifySignatureDelete(UpdateSignatureOtpRequest req)
        {
            try
            {
                _logger.LogInformation($"Entered VerifySignatureDelete");
                SessionContext sessionContext = GetSessionContext();
                var user = _cardiologistRepository.GetByEmail(req.Email);
                var principle = JwtWrapper.GetClaimsPrincipal(ConfigurationConsts.DeleteSignatureVerificationKey.Sha256(), req.Token);
                if (principle?.Identity?.IsAuthenticated == true)
                {
                    _logger.LogInformation($"VerifySignatureDelete: user identity is authenticated");
                    var claimsIdentity = principle.Identity as ClaimsIdentity;
                    var claims = claimsIdentity.Claims.ToArray();
                    var sid = claims.Single(c => c.Type == "sid").Value;
                    var hash = claims.Single(c => c.Type == "otp_hash").Value;
                    if (hash == string.Format("{0}:{1}", sid, req.OTP.ToString()).Sha256() &&
                        claims.Single(c => c.Type == "otp_email").Value == req.Email)
                    {
                        using (var _unitOfWork = _serviceProvider.GetRequiredService<Common.DataAccess.Interfaces.IUnitOfWork>())
                        {
                            try
                            {
                                var sig = _signaturesRepository.GetByCardiologist(sessionContext.AppUserID);
                                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.FileSharingServiceBaseUrl, TimeSpan.FromMinutes(15)))
                                {
                                    if (sig == null)
                                        throw new ServiceException(ClientSideErrors.INVALID_USER_ACTION);
                                    try
                                    {
                                        var res1 = apiClient.DeleteInternalAsync($"api/v1/filesharing/{sig.FilePath}").Result;
                                    }
                                    catch (Exception se)
                                    {
                                        if (se.InnerException != null && se.InnerException is ServiceException && se.InnerException.Message == ClientSideErrors.RESOURCE_NOT_FOUND)
                                        {
                                        }
                                        else throw;
                                    }                                    
                                    _signaturesRepository.Delete(sig);
                                    _unitOfWork.Commit();
                                }
                                return;
                            }
                            catch (Exception ex)
                            {
                                _unitOfWork.Rollback();
                                _logger.LogError(ex, ex.Message);
                                throw;
                            }
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"VerifySignatureDelete: OTP or email is not matched");

                    }
                }
                else
                {
                    _logger.LogInformation($"VerifySignatureDelete: user identity is not authenticated");
                }
            }
            catch
            {
            }
            throw new ServiceException(ClientSideErrors.OTP_VERIFICATION_FAILED);
        }
    }
}
