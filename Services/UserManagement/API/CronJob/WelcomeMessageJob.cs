using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.BusinessObjects;
using Common.DataAccess.Models;
using Common.Infrastructure;
using Common.Infrastructure.Extensions;
using Common.Interfaces.Interfaces;
using Common.Services.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UserManagement.Contracts.Interfaces;
using UserManagement.Repository.Interfaces;

namespace UserManagement.API.CronJob
{
    public class WelcomeMessageJob : CronJobService
    {
        private readonly ILogger<WelcomeMessageJob> _logger;
        private readonly IServiceProvider _serviceProvider;

        public WelcomeMessageJob(ICronConfig<WelcomeMessageJob> config, ILogger<WelcomeMessageJob> logger, IServiceProvider serviceProvider)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(StartAsync)}: About to schedule welcome message job.");
            return base.StartAsync(cancellationToken);
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} About to send welcome message to users.");
            try
            {
                var scope = _serviceProvider.CreateScope();
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var credentialRepository = scope.ServiceProvider.GetRequiredService<ICredentialRepository>();
                var userRoleRepository = scope.ServiceProvider.GetRequiredService<IUserRoleRepository>();
                var roleRepository = scope.ServiceProvider.GetRequiredService<IRoleRepository>();
                var applicationRepository = scope.ServiceProvider.GetRequiredService<IApplicationRepository>();
                var rabbitMqClient = scope.ServiceProvider.GetRequiredService<RabbitMQClient>();
                
                var users = userRepository.GetUserForWelcomeMessage().ToList();

                foreach (var user in users)
                {
                    try
                    {
                        //TODO: Will write one query for this later
                        var credentials = credentialRepository.GetByID(user.ID);
                        var userRole = userRoleRepository.GetByUserID(user.ID);
                        var role = roleRepository.GetByID(userRole.RoleID);
                        var application = applicationRepository.GetByID(role.ApplicationID);
                        _logger.LogInformation($"{DateTime.Now:hh:mm:ss} About to send welcome message email to {user.LoginName}.");
                        rabbitMqClient.SendMessage(KnownChannels.USER_REGISTER_EVENT_CHANNEL, new BaseUserBO()
                        {
                            ApplicationCode = application.Code,
                            LoginName = user.LoginName,
                            Password = credentials.Password.DecodeBase64(),
                            Role = role.Code
                        });

                        user.IsWelcomeMessageRequired = false;
                        userRepository.Update(user);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"{nameof(DoWork)}: Error occurred while sending email to {user.LoginName}. Error: {e.Message}");
                    }
                   
                }


            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(DoWork)}: Error occurred while sending welcome message email. Error: {e.Message}");
            }

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(StopAsync)}: About to stop welocme message job.");
            return base.StopAsync(cancellationToken);
        }
    }
}
