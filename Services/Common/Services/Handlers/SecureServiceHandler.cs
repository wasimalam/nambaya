using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Services.Handlers
{
    public class SecureServiceHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly System.IServiceProvider _serviceProvider;
        private readonly ILogger<SecureServiceHandler> _logger;
        public SecureServiceHandler(IHttpContextAccessor httpContextAccessor, System.IServiceProvider serviceProvider)
        {
            this._httpContextAccessor = httpContextAccessor;
            _serviceProvider = serviceProvider;
            _logger = serviceProvider.GetRequiredService<ILogger<SecureServiceHandler>>();
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", GetAccessToken());
            return base.SendAsync(request, cancellationToken);
        }
        private string GetAccessToken()
        {
            try
            {
                var context = this._httpContextAccessor.HttpContext;
                WebServiceConfiguration webServiceConfiguration = _serviceProvider.GetRequiredService<WebServiceConfiguration>();
                ApiRequestConfiguration apiSecretConf = _serviceProvider.GetRequiredService<ApiRequestConfiguration>();
                var result = ApiClient.GetAccessToken(webServiceConfiguration.IdentityServerBaseUrl, apiSecretConf);
                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                throw new ServiceException("Unable to get AccessToken");
            }
        }
    }
}
