using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Common.Services.Handlers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Common.Services
{
    public class WebApiClient : ApiClient
    {
        private readonly IServiceProvider _serviceProvider;
        private DelegatingHandler _requesthandler;
        private DelegatingHandler _securityhandler;
        public WebApiClient(IServiceProvider serviceProvider, string baseUrl, TimeSpan? timeout = null) : base(baseUrl, timeout)
        {
            _serviceProvider = serviceProvider;
            _requesthandler = _serviceProvider.GetRequiredService<RequestHandler>();
            _securityhandler = _serviceProvider.GetRequiredService<SecureServiceHandler>();
        }
        protected override void CreateHttpClient()
        {
            _httpClientHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            };
            _httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            _httpClient = HttpClientFactory.Create(_httpClientHandler, _requesthandler, _securityhandler);
            _httpClient.Timeout = _timeout;

            if (!string.IsNullOrWhiteSpace(_baseUrl))
            {
                _httpClient.BaseAddress = new Uri(_baseUrl);
            }
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeJson));
        }
        public new void Dispose()
        {
            _requesthandler?.Dispose();
            _securityhandler?.Dispose();
            _httpClientHandler?.Dispose();
            _httpClientHandler = null;
            _httpClient?.Dispose();
            _httpClient = null;
        }
        public new async Task<string> InternalServicePostAsync(string url, string input, string contenttype = MediaTypeJson)
        {
            int retry = 1;
            do
            {
                try
                {
                    return await base.InternalServicePostAsync(url, input, contenttype);
                }
                catch (ServiceException ex)
                {
                    if (ex.Message == ClientSideErrors.AUTHENTICATION_RETRY && retry > 0)
                        continue;
                    throw ex;
                }
                catch
                {
                    throw;
                }
            } while (retry-- != 0);
            return "";
        }
        public new async Task<string> InternalServicePostFileAsync(string url, string filename, byte[] data, object obj, string fileContentType = MediaTypeOctet, string dataContentType = MediaTypeJson)
        {
            int retry = 1;
            do
            {
                try
                {
                    return await base.InternalServicePostFileAsync(url, filename, data, obj, fileContentType, dataContentType);
                }
                catch (ServiceException ex)
                {
                    if (ex.Message == ClientSideErrors.AUTHENTICATION_RETRY && retry > 0)
                        continue;
                    throw ex;
                }
                catch
                {
                    throw;
                }
            } while (retry-- != 0);
            return "";
        }
        public new async Task<string> InternalServiceGetAsync(string url)
        {
            int retry = 1;
            do
            {
                try
                {
                    return await base.InternalServiceGetAsync(url);
                }
                catch (ServiceException ex)
                {
                    if (ex.Message == ClientSideErrors.AUTHENTICATION_RETRY && retry > 0)
                        continue;
                    throw ex;
                }
                catch
                {
                    throw;
                }
            } while (retry-- != 0);
            return "";
        }
        public new async Task<string> InternalServiceGetAsync(string url, string bodyContent)
        {
            int retry = 1;
            do
            {
                try
                {
                    return await base.InternalServiceGetAsync(url, bodyContent);
                }
                catch (ServiceException ex)
                {
                    if (ex.Message == ClientSideErrors.AUTHENTICATION_RETRY && retry > 0)
                        continue;
                    throw ex;
                }
                catch
                {
                    throw;
                }
            } while (retry-- != 0);
            return "";
        }
        public new async Task<string> InternalServicePutAsync(string url, object input)
        {
            int retry = 1;
            do
            {
                try
                {
                    return await base.InternalServicePutAsync(url, input);
                }
                catch (ServiceException ex)
                {
                    if (ex.Message == ClientSideErrors.AUTHENTICATION_RETRY && retry > 0)
                        continue;
                    throw ex;
                }
                catch
                {
                    throw;
                }
            } while (retry-- != 0);
            return "";
        }
    }
}
