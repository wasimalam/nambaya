using AutoMapper;
using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Common.Infrastructure.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System;
using UserManagement.Contracts.Interfaces;
using Microsoft.Extensions.Logging;
using UserManagement.Contracts.Models;
using UserManagement.Repository.Interfaces;
using UserManagement.Repository.Models;

namespace UserManagement.Service
{
    public class CredentialService : ICredentialService
    {
        private readonly ICredentialRepository _credentialRepository;
        private readonly ICredentialHistoryRepository _credentialHistoryRepository;
        private readonly IMapper _mapper;
        private readonly UserLoginPolicy _userLoginPolicy;
        private readonly ILogger<CredentialService> _logger;
        private readonly IServiceProvider _serviceProvider;
        public CredentialService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _credentialRepository = serviceProvider.GetRequiredService<ICredentialRepository>();
            _credentialHistoryRepository = serviceProvider.GetRequiredService<ICredentialHistoryRepository>();
            _mapper = serviceProvider.GetRequiredService<IMapper>();
            _userLoginPolicy = serviceProvider.GetRequiredService<UserLoginPolicy>();
            _logger = serviceProvider.GetRequiredService<ILogger<CredentialService>>();
        }
        public CredentialBO UpdateUserPassword(CredentialBO cred)
        {
            _logger.LogInformation("Update user password started");
            //using (var _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                try
                {
                    _logger.LogDebug($"Getting credentials against user id {cred.UserID}");
                    var credential = _credentialRepository.GetByID(cred.UserID);
                    if (PasswordGenerator.ValidPasssword(_userLoginPolicy, cred.Password) == false)
                        throw new ServiceException(ClientSideErrors.POLICY_INVALID_PASSWORD);
                    if (credential != null)
                    {
                        _credentialHistoryRepository.Insert(new CredentialHistory()
                        {
                            IsDeleted = credential.IsDeleted,
                            UserID = credential.UserID,
                            Password = credential.Password,
                            CreatedBy = credential.CreatedBy
                        });
                        credential.Password = cred.Password;
                        credential.UpdatedBy = cred.UpdatedBy;
                        _credentialRepository.Update(credential);
                        //_unitOfWork.Commit();
                        return _mapper.Map<CredentialBO>(credential);
                    }
                    else
                    {
                        _logger.LogWarning("Credentials not found against user");
                    }
                }
                catch
                {
                    //_unitOfWork.Rollback();
                    throw;
                }
            }
            throw new ServiceException(ClientSideErrors.INVALID_USER_ID);
        }
    }
}
