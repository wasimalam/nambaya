using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;

namespace Common.Infrastructure.Extensions
{
    public static class ApiHelper
    {

        /// <summary>
        /// When applied to a <see cref="HttpContext"/>, verifies that the user authenticated in the
        /// web API has any of the accepted scopes.
        /// If the authenticated user doesn't have any of these <paramref name="acceptedScopes"/>, the
        /// method throws an HTTP Unauthorized error with a message noting which scopes are expected in the token.
        /// </summary>
        /// <param name="acceptedScopes">Scopes accepted by this API</param>
        /// <exception cref="HttpRequestException"/> with a <see cref="HttpResponse.StatusCode"/> set to 
        /// <see cref="HttpStatusCode.Unauthorized"/>
        public static void VerifyUserHasAnyAcceptedScope(this HttpContext context,
                                                         params string[] acceptedScopes)
        {
            if (acceptedScopes == null)
            {
                throw new ArgumentNullException(nameof(acceptedScopes));
            }
            Claim scopeClaim = context?.User?.FindFirst("scope");
            if (scopeClaim == null || !scopeClaim.Value.Split(' ').Intersect(acceptedScopes).Any())
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                string message = $"The 'scope' claim does not contain scopes '{string.Join(",", acceptedScopes)}' or was not found";
                throw new HttpRequestException(message);
            }
        }
        public static void AddMassTransitSupport(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitmqsetting = new RabbitMQSettings();
            configuration.GetSection(ConfigurationConsts.RabbitMQSettings).Bind(rabbitmqsetting);
            services.AddSingleton(rabbitmqsetting);
            services.AddMassTransit(x =>
            {
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var host = cfg.Host(new Uri($"rabbitmq://{rabbitmqsetting.Host}:{rabbitmqsetting.Port}"), rabbitmqsetting.ConnectionName, hostConfig =>
                    {
                        hostConfig.Username(rabbitmqsetting.Username);
                        hostConfig.Password(rabbitmqsetting.Password);
                    });
                }));
            });
        }

        public static void AddRabbitMQSettings(this IServiceCollection services, IConfiguration configuration)
        {
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            var Configuration = serviceProvider.GetService<IConfiguration>();
            var rabbitmqsetting = new RabbitMQSettings();
            Configuration.GetSection(ConfigurationConsts.RabbitMQSettings).Bind(rabbitmqsetting);
            services.AddSingleton(rabbitmqsetting);
        }
        public static void AddWebServiceConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            WebServiceConfiguration webApiConf = new WebServiceConfiguration();
            configuration.GetSection(ConfigurationConsts.WebApiConfigurationKey).Bind(webApiConf);
            services.AddSingleton(_ => webApiConf);
        }
        public static ApiRequestConfiguration AddApiRequestConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var apiSecretConf = new ApiRequestConfiguration();
            configuration.GetSection(ConfigurationConsts.ApiSecretConfiguration).Bind(apiSecretConf);
            services.AddSingleton(_ => apiSecretConf);
            return apiSecretConf;
        }
        public static ApiConfiguration AddApiAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            ApiConfiguration apiConf = new ApiConfiguration();
            configuration.GetSection(ConfigurationConsts.ApiConfigurationKey).Bind(apiConf);
            services.AddSingleton(_ => apiConf);
            services.AddAuthentication("Bearer")
            .AddIdentityServerAuthentication(options =>
             {
                 options.Authority = apiConf.IdentityServerBaseUrl;
                 options.RequireHttpsMetadata = apiConf.RequireHttpsMetadata;
                 options.ApiName = apiConf.OidcApiName;
             });
            return apiConf;
        }
        public static void AddCustomKeyStoreProvider(this IServiceCollection services, IConfiguration configuration)
        {
            DBCustomEncryption dbEncryptionConf = new DBCustomEncryption();
            configuration.GetSection(ConfigurationConsts.DBCustomEncryption).Bind(dbEncryptionConf);
            services.AddSingleton(_ => dbEncryptionConf);
            services.AddSingleton(new CustomKeyStoreProvider(dbEncryptionConf));
        }
        public static void AddUserLoginPolicy(this IServiceCollection services, IConfiguration configuration)
        {
            UserLoginPolicy dbEncryptionConf = new UserLoginPolicy();
            configuration.GetSection(ConfigurationConsts.UserLoginPolicy).Bind(dbEncryptionConf);
            services.AddSingleton(_ => dbEncryptionConf);
        }
    }
}
