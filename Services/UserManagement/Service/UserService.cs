using AutoMapper;
using Common.BusinessObjects;
using Common.DataAccess.Interfaces;
using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Common.Infrastructure.Extensions;
using Microsoft.Extensions.Logging;
using Common.Infrastructure.Helpers;
using Common.Services;
using LazyCache;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using UserManagement.Contracts.Interfaces;
using UserManagement.Contracts.Models;
using UserManagement.Repository.Interfaces;
using UserManagement.Repository.Models;

namespace UserManagement.Service
{
    public class UserService : BaseService, IUserService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ICredentialRepository _credentialRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly ILogger<UserService> _logger;
        private readonly IAppCache _cache;
        private readonly IUserSettingRepository _userSettingRepository;
        private readonly IMapper _mapper;
        private readonly ICredentialService _credentialService;
        private readonly RabbitMQClient _rabbitMQClient;
        private readonly UserLoginPolicy _userLoginPolicy;
        public UserService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _mapper = serviceProvider.GetRequiredService<IMapper>();
            _applicationRepository = serviceProvider.GetRequiredService<IApplicationRepository>();
            _userRepository = serviceProvider.GetRequiredService<IUserRepository>();
            _roleRepository = serviceProvider.GetRequiredService<IRoleRepository>();
            _userRoleRepository = serviceProvider.GetRequiredService<IUserRoleRepository>();
            _credentialRepository = serviceProvider.GetRequiredService<ICredentialRepository>();
            _credentialService = serviceProvider.GetRequiredService<ICredentialService>();
            _userSettingRepository = serviceProvider.GetRequiredService<IUserSettingRepository>();
            _rabbitMQClient = serviceProvider.GetRequiredService<RabbitMQClient>();
            _userLoginPolicy = serviceProvider.GetRequiredService<UserLoginPolicy>();
            _cache = serviceProvider.GetRequiredService<IAppCache>();
            _logger = serviceProvider.GetRequiredService<ILogger<UserService>>();

        }

        PagedResults<UserBO> IUserService.GetUsers(int limit, int offset, string orderby, string param)
        {
            _logger.LogInformation($"GetUsers limit {limit} offset {offset} orderby {orderby} param {param}");
            PagedResults<UserBO> pg = new PagedResults<UserBO>();
            IFilter filter = null;
            if (!string.IsNullOrWhiteSpace(param))
            {
                var expressions = Expression.StringToExpressions(param);
                var filterlist = expressions.Select(p => new Filter(p.Property, p.Operation, p.Value)).ToList();
                filter = new ANDFilter(filterlist.Select(p => p as IFilter).ToList());
            }
            var pdb = _userRepository.GetAllPages(limit, offset, orderby, filter);
            pg.TotalCount = pdb.TotalCount;
            pg.PageSize = pdb.PageSize;
            _logger.LogInformation($"GetUsers TotalCount {pg.TotalCount} pagesize {pg.PageSize}");
            pg.Data = pdb.Data.Select(p => _mapper.Map<UserBO>(p)).ToList();
            return pg;
        }

        public UserBO GetUserById(long id)
        {
            return _mapper.Map<UserBO>(_userRepository.GetByID(id));
        }

        public SessionContext GetUserSessionById(long id)
        {
            _logger.LogInformation($"GetUserSessionById: id {id}");
            var user = _userRepository.GetByID(id);
            var usersession = _mapper.Map<SessionContext>(user);
            var ur = _userRoleRepository.GetByUserID(user.ID);
            var role = _roleRepository.GetByID(ur.RoleID);
            usersession.ApplicationCode = _applicationRepository.GetByID(role.ApplicationID).Code;
            _logger.LogInformation($"GetUserSessionById: user role {role.Code}");
            usersession.RoleCode = role.Code;
            return usersession;
        }
        public SessionContext GetUserSessionByLoginId(string loginname)
        {
            _logger.LogInformation($"GetUserSessionByLoginId: loginname {loginname}");

            var user = _userRepository.GetByLoginName(loginname);
            var usersession = _mapper.Map<SessionContext>(user);
            var ur = _userRoleRepository.GetByUserID(user.ID);
            var role = _roleRepository.GetByID(ur.RoleID);
            usersession.ApplicationCode = _applicationRepository.GetByID(role.ApplicationID).Code;
            usersession.RoleCode = role.Code;
            _logger.LogInformation($"GetUserSessionByLoginId: user role {role.Code}");

            return usersession;
        }
        UserBO IUserService.GetUserByLoginName(string loginname)
        {
            _logger.LogInformation($"GetUserByLoginName: loginname {loginname}");

            var user = _mapper.Map<UserBO>(_userRepository.GetByLoginName(loginname));
            if (user != null)
            {
                var role = _roleRepository.GetByLoginName(loginname);
                user.Role = role.Code;
                _logger.LogInformation($"GetUserByLoginName: user role {role.Code}");

            }
            else
            {
                _logger.LogInformation("GetUserByLoginName: user not found");
            }
            return user;
        }

        List<UserBO> IUserService.GetUsersByLoginName(string[] loginnames)
        {
            var users = _userRepository.GetByLoginName(new List<string>(loginnames));
            return users.Select(p => _mapper.Map<UserBO>(p)).ToList();
        }

        public List<UserBO> GetUsersByRole(string role, bool? isActive, bool? isLocked)
        {
            _logger.LogInformation($"GetUsersByRole: role {role}");
            var users = _userRepository.GetByRoleCode(role, isActive, isLocked);
            return users.Select(p => _mapper.Map<UserBO>(p)).ToList();
        }

        SessionContext IUserService.IsValidUser(string loginId, string password, bool bLoginAttempt)
        {
            _logger.LogInformation($"IsValidUser: login id {loginId}");
            var user = _userRepository.GetByLoginName(loginId);
            if (user != null)
            {
                if (user.IsLocked)
                    throw new ServiceException(ClientSideErrors.USER_ID_LOCKED);
                if (_credentialRepository.IsValid(user.ID, password))
                {
                    _logger.LogInformation("User is valid");
                    if (!user.IsActive)
                        throw new ServiceException(ClientSideErrors.USER_ID_INACTIVE);
                    else if (user.IsDeleted)
                        throw new ServiceException(ClientSideErrors.USER_ID_DELETED);
                    else if (user.IsPasswordResetRequired)
                        throw new ServiceException(ClientSideErrors.PASSWORD_RESET_REQUIRED);

                    if (bLoginAttempt)
                    {
                        user.LastLoggedInOn = DateTime.UtcNow;
                        user.PasswordAttempts = 0;
                        _userRepository.Update(user);
                    }
                    var usersession = _mapper.Map<SessionContext>(user);
                    var ur = _userRoleRepository.GetByUserID(user.ID);
                    var role = _roleRepository.GetByID(ur.RoleID);
                    usersession.ApplicationCode = _applicationRepository.GetByID(role.ApplicationID).Code;
                    usersession.RoleCode = role.Code;
                    _logger.LogInformation($"isValidUser: application code {usersession.ApplicationCode} role code {usersession.RoleCode}");
                    return usersession;
                }
                else if (bLoginAttempt)
                {
                    user.PasswordAttempts = user.PasswordAttempts + 1;
                    // Needs to set user.IsLocked = 1 here on some policy
                    if (_userLoginPolicy.MaxLoginAttempts < user.PasswordAttempts)
                        user.IsLocked = true;
                    _userRepository.Update(user);
                    if (user.IsLocked)
                        throw new ServiceException(ClientSideErrors.USER_ID_LOCKED);
                }
            }
            else
            {
                _logger.LogWarning("IsValidUser: User not found");
            }
            return null;
        }

        public UserBO CreateUser(BaseUserBO baseUser)
        {
            _logger.LogInformation("Create User started");
            using (var unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                try
                {
                    SessionContext sessionContext = GetSessionContext();
                    if (sessionContext != null)
                        baseUser.CreatedBy = sessionContext.LoginName;
                    baseUser.Password = PasswordGenerator.GeneratePassword(_userLoginPolicy);
                    var user = _userRepository.GetByLoginName(baseUser.LoginName);
                    if (user == null)
                    {
                        _logger.LogInformation("CreateUser: Creating user ");
                        user = new Repository.Models.User()
                        {
                            LoginName = baseUser.LoginName,
                            IsActive = baseUser.IsActive,
                            IsDomainUser = false,
                            IsPasswordResetRequired = true,
                            IsLocked = false,
                            PasswordAttempts = 0,
                            IsDeleted = false,
                            CreatedBy = baseUser.CreatedBy
                        };
                        _userRepository.Insert(user);

                        _credentialRepository.InsertPassword(new Credential()
                        {
                            UserID = user.ID,
                            Password = baseUser.Password.Base64(),
                            IsDeleted = false,
                            CreatedBy = baseUser.CreatedBy
                        });
                        var roleObj = _roleRepository.GetRole(baseUser.ApplicationCode, baseUser.Role);
                        if (roleObj == null)
                            throw new ServiceException(ClientSideErrors.INVALID_USER_APPLICATION_ROLE);
                        _logger.LogInformation("CreateUser: Adding user role");
                        _userRoleRepository.Insert(
                            new UserRole()
                            {
                                RoleID = roleObj.ID,
                                UserID = user.ID,
                                CreatedBy = baseUser.CreatedBy
                            });
                        _logger.LogInformation("CreateUser: Adding user settings");

                        _userSettingRepository.Insert(
                            new UserSetting()
                            {
                                UserId = user.ID,
                                ApplicationID = roleObj.ApplicationID,
                                Code = "Language",
                                Value = LanguageCode.English,
                                DataType = "Language",
                                CreatedBy = baseUser.CreatedBy
                            });
                        _userSettingRepository.Insert(
                           new UserSetting()
                           {
                               UserId = user.ID,
                               ApplicationID = roleObj.ApplicationID,
                               Code = "2FactorNotificationType",
                               Value = NotificationType.Email.ToString(),
                               DataType = "Lookups",
                               CreatedBy = baseUser.CreatedBy
                           });
                        unitOfWork.Commit();
                        _rabbitMQClient.SendMessage(KnownChannels.USER_REGISTER_EVENT_CHANNEL, baseUser);
                        ClearRoleCache(baseUser.Role);
                        _logger.LogInformation($"CreateUser: User sucessfully created");
                        return _mapper.Map<UserBO>(user);
                    }
                    else
                        throw new ServiceException(ClientSideErrors.USER_ID_ALREADY_EXISTS);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    unitOfWork.Rollback();
                    throw;
                }
            }
        }

        public void UpdateUser(BaseUserBO baseUser)
        {
            _logger.LogInformation("Updating user started");
            using (var unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                try
                {
                    var sessionContext = GetSessionContext();
                    if (sessionContext != null)
                        baseUser.UpdatedBy = sessionContext.LoginName;
                    var user = _userRepository.GetByLoginName(baseUser.LoginName);
                    if (user != null)
                    {
                        if (string.IsNullOrWhiteSpace(baseUser.Password) == false)
                        {
                            _logger.LogInformation($"UpdateUser: Updating user password");
                            _credentialService.UpdateUserPassword(
                            //var cred =
                            new CredentialBO()
                            {
                                UserID = user.ID,
                                Password = baseUser.Password,
                                UpdatedBy = baseUser.UpdatedBy
                            });
                        }
                        if ((sessionContext.RoleCode == RoleCodes.NambayaUser ||
                            sessionContext.RoleCode == RoleCodes.CentralGroupUser) ||
                            (sessionContext.RoleCode == RoleCodes.PharmacyTrainer && baseUser.Role == RoleCodes.Pharmacy) ||
                            (sessionContext.RoleCode == RoleCodes.Pharmacy && baseUser.Role == RoleCodes.Pharmacist)||
                            (sessionContext.RoleCode == RoleCodes.Cardiologist && baseUser.Role == RoleCodes.Nurse)
                            )
                        {
                            if ((sessionContext.RoleCode == RoleCodes.NambayaUser)
                                && sessionContext.LoginName.ToLower() != baseUser.LoginName.ToLower()
                                && string.IsNullOrWhiteSpace(baseUser.Role) == false)
                            {
                                var tempUser = _userRepository.GetByLoginName(baseUser.LoginName);
                                var roleObj = _roleRepository.GetRole(baseUser.ApplicationCode, baseUser.Role);
                                if (tempUser.Role.ToLower() != roleObj.Code.ToLower())
                                {
                                    _logger.LogInformation($"UpdateUser: Updating user roles");
                                    _userRoleRepository.UpdateUserRole(new UserRole()
                                    {
                                        RoleID = roleObj.ID,
                                        UserID = user.ID,
                                        UpdatedBy = baseUser.UpdatedBy
                                    });
                                    ClearRoleCache(baseUser.Role);
                                    ClearRoleCache(tempUser.Role);
                                }
                            }
                            if (user.IsActive != baseUser.IsActive || user.IsLocked != baseUser.IsLocked)
                            {
                                _logger.LogInformation("UpdateUser: updating user settings ");
                                user.IsActive = baseUser.IsActive;
                                if(user.IsLocked && baseUser.IsLocked == false) user.PasswordAttempts = 0;
                                user.IsLocked = baseUser.IsLocked;

                                user.UpdatedBy = baseUser.UpdatedBy;
                                _userRepository.Update(user);
                                ClearRoleCache(baseUser.Role);
                            }
                        }
                        unitOfWork.Commit();
                        _logger.LogInformation("Updating user completed");
                        return;
                    }
                    else
                        throw new ServiceException(ClientSideErrors.INVALID_USER_ID);
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }
            }
        }
        void IUserService.UpdateUserCredentials(BaseUserBO baseUser)
        {
            using (var _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                try
                {
                    var sessionContext = GetSessionContext();
                    if (sessionContext != null)
                        baseUser.UpdatedBy = sessionContext.LoginName;
                    var user = _userRepository.GetByLoginName(baseUser.LoginName);
                    if (user != null)
                    {
                        _logger.LogInformation($"UpdateUserCredentials for user id {user.ID}");
                        _credentialService.UpdateUserPassword(new CredentialBO()
                        {
                            UserID = user.ID,
                            Password = baseUser.Password.Base64(),
                            UpdatedBy = baseUser.UpdatedBy
                        });
                        user.IsPasswordResetRequired = false;
                        _userRepository.Update(user);
                        _unitOfWork.Commit();
                        return;
                    }
                    throw new ServiceException(ClientSideErrors.INVALID_USER_ID);
                }
                catch
                {
                    _unitOfWork.Rollback();
                    throw;
                }
            }
        }

        List<UserSettingBO> IUserService.GetUserSettings(string loginid)
        {
            _logger.LogInformation($"Getting user setting of login Id {loginid}");
            var user = _userRepository.GetByLoginName(loginid);
            if (user != null)
            {
                var settings = _userSettingRepository.GetByUserID(user.ID);

                if (settings == null || settings.Any() == false)
                {
                    settings = new List<UserSetting>() {
                    new UserSetting()
                    {
                        Code = UserSettingCodes.LANGUAGE,
                        Value = LanguageCode.English,
                        DataType = UserSettingDataTypes.LANGUAGE,
                    },
                    new UserSetting()
                    {
                        Code = UserSettingCodes.FACTOR_NOTIFICATION_TYPE,
                         Value = NotificationType.Email.ToString(),
                         DataType = UserSettingDataTypes.LOOKUPS
                    }
                    };
                }
                else if (settings.Any(p => p.Code == UserSettingCodes.LANGUAGE) == false)
                {
                    settings = settings.Append(new UserSetting()
                    {
                        Code = UserSettingCodes.LANGUAGE,
                        Value = LanguageCode.English,
                        DataType = UserSettingDataTypes.LANGUAGE,
                    });
                }

                return settings.Select(p => _mapper.Map<UserSettingBO>(p)).ToList();
            }
            throw new ServiceException(ClientSideErrors.INVALID_USER_ID);
        }
        void IUserService.UpateUserSettings(string loginid, IEnumerable<UserSettingBO> userSettings)
        {
            _logger.LogInformation($"UpdateUserSettinsgs: LoginId {loginid}");
            var user = _userRepository.GetByLoginName(loginid);
            foreach (var u in userSettings)
            {
                var setting = _userSettingRepository.GetByCode(user.ID, u.Code);
                if (setting != null)
                {
                    setting.Value = u.Value;
                    setting.UpdatedBy = loginid;
                    _userSettingRepository.Update(setting);
                }
                else
                {
                    var role = _roleRepository.GetByLoginName(loginid);
                    _userSettingRepository.Insert(new UserSetting
                    {
                        ApplicationID = role.ApplicationID,
                        Code = u.Code,
                        CreatedBy = loginid,
                        DataType = u.DataType,
                        UserId = user.ID,
                        Value = u.Value
                    });
                }
            }
        }

        public void ChangeCredentials(string loginId, ChangeCredentialBO req)
        {
            _logger.LogInformation($"ChangeCredentials for loginId {loginId}");
            var user = _userRepository.GetByID(req.UserId);

            if (user == null)
                throw new ServiceException(ClientSideErrors.INVALID_USER_ID);

            var userCredentials = _credentialRepository.GetByID(user.ID);

            if (userCredentials.Password == req.OldPassword.Base64())
            {
                userCredentials.Password = req.NewPassword.Base64();
                userCredentials.UpdatedBy = loginId;
                _logger.LogInformation("Updating credenitals ");
                _credentialRepository.Update(userCredentials);
            }
            else
            {
                throw new ServiceException(ClientSideErrors.OLD_PASSWORD_DOES_NOT_MATCH);
            }

        }
        private void ClearRoleCache(string role)
        {
            _cache.Remove($"{role}__{null}__{null}");
            _cache.Remove($"{role}__{null}__{true}");
            _cache.Remove($"{role}__{null}__{false}");
            _cache.Remove($"{role}__{true}__{null}");
            _cache.Remove($"{role}__{true}__{true}");
            _cache.Remove($"{role}__{true}__{false}");
            _cache.Remove($"{role}__{false}__{null}");
            _cache.Remove($"{role}__{false}__{true}");
            _cache.Remove($"{role}__{false}__{false}");
        }
    }
}