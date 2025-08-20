using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Nambaya.AEGnordNotifier.Logger;
using Nambaya.AEGnordNotifier.Models;

namespace Nambaya.AEGnordNotifier.Service
{
    public class WebApiClientService : IDisposable
    {
        private HttpClient _client;

        public WebApiClientService()
        {
            CreateHttpClient();
        }

        private void CreateHttpClient()
        {
            _client = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            });
        }
        public async Task<string> GetAuthorizeToken(ApiRequestConfiguration configuration,string url)
        {
            try
            {
                EnsureHttpClientCreated();
                var discoveryDocument = await _client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
                {
                    
                    Address = url,
                    Policy =
                    {
                        RequireHttps = false,
                        ValidateIssuerName = false,
                        ValidateEndpoints = false,
                        
                    }
                    

                });

                if (discoveryDocument.IsError) throw new Exception("Could not get discovery document.");

                var tokenResponse = await _client.RequestClientCredentialsTokenAsync(
                    new ClientCredentialsTokenRequest
                    {
                        Address = discoveryDocument.TokenEndpoint,
                        ClientId = configuration.ClientId,
                        ClientSecret = configuration.Secret,
                        Scope = configuration.Scope

                        //ClientId = "patient_api_client",
                        //ClientSecret = "ECC880C5-E3B9-4EAB-8595-147957BADF48",
                        //Scope = "file_sharing_api_full file_sharing_api_read",
                    });

                if (tokenResponse.IsError)
                    throw new Exception("Could not get access token. Please check identity server configurations.");

                return tokenResponse.AccessToken;

            }
            catch (Exception e)
            {
                LogHelper.Log.Error($"{nameof(GetAuthorizeToken)}: Some error occurred during get access token. Error detail: {e}");
                throw;
            }

        }

        public async Task<byte[]> DownloadGetAsync(string url, string contentType, string acceptType)
        {
            try
            {
                EnsureHttpClientCreated();
                if (!string.IsNullOrWhiteSpace(acceptType))
                    _client.DefaultRequestHeaders.Add("Accept", acceptType);
                using (var response = await _client.GetAsync(url))
                {
                    if (!string.IsNullOrWhiteSpace(acceptType))
                        _client.DefaultRequestHeaders.Remove("Accept");
                    if (response.StatusCode == HttpStatusCode.BadRequest)
                        throw new Exception(response.Content.ReadAsStringAsync().Result.Replace("\"", ""));
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsByteArrayAsync();
                }
            }
            catch (Exception e)
            {
                LogHelper.Log.Error($"{nameof(DownloadGetAsync)}: Failed to get file bytes from API. Error detail: {e}");
                throw;
            }
            
        }

        public void SetBearerToken(string accessToken)
        {
            EnsureHttpClientCreated();
            _client.SetBearerToken(accessToken);
        }
        private void EnsureHttpClientCreated()
        {
            if (_client == null)
            {
                CreateHttpClient();
            }
        }
        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
