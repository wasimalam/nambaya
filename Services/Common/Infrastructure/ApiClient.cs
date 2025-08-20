using Common.Infrastructure.Exceptions;
using IdentityModel.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Common.Infrastructure
{
    public class ApiClient : IDisposable
    {
        protected TimeSpan _timeout;
        protected HttpClient _httpClient;
        protected HttpClientHandler _httpClientHandler;
        protected readonly string _baseUrl;
        protected const string MediaTypeJson = "application/json";
        protected const string MediaTypeOctet = "application/octet-stream";

        private static ConcurrentDictionary<string, AccessTokenItem> _accessTokens = new ConcurrentDictionary<string, AccessTokenItem>();
        private static ConcurrentDictionary<string, ApiRequestConfiguration> _accessTokenConfiguration = new ConcurrentDictionary<string, ApiRequestConfiguration>();

        private class AccessTokenItem
        {
            public string AccessToken { get; set; } = string.Empty;
            public DateTime ExpiresIn { get; set; }
        }

        public ApiClient(string baseUrl, TimeSpan? timeout = null)
        {
            _baseUrl = NormalizeBaseUrl(baseUrl);
            _timeout = timeout ?? TimeSpan.FromSeconds(90);
        }

        public async Task<string> PostAsync(string url, string input, string contenttype = MediaTypeJson)
        {
            EnsureHttpClientCreated();

            using (var requestContent = new StringContent(input, Encoding.UTF8, contenttype))
            {
                using (var response = await _httpClient.PostAsync(url, requestContent))
                {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }

        public async Task<byte[]> DownloadAsync(string url, string input, string contenttype = MediaTypeJson, string acceptType = "")
        {
            EnsureHttpClientCreated();

            using (var requestContent = new StringContent(input, Encoding.UTF8, contenttype))
            {
                if (!string.IsNullOrWhiteSpace(acceptType))
                    _httpClient.DefaultRequestHeaders.Add("Accept", acceptType);
                using (var response = await _httpClient.PostAsync(url, requestContent))
                {
                    if (!string.IsNullOrWhiteSpace(acceptType))
                        _httpClient.DefaultRequestHeaders.Remove("Accept");
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsByteArrayAsync();
                }
            }
        }

        public async Task<byte[]> DownloadGetAsync(string url, string contenttype = MediaTypeJson, string acceptType = "")
        {
            EnsureHttpClientCreated();
            if (!string.IsNullOrWhiteSpace(acceptType))
                _httpClient.DefaultRequestHeaders.Add("Accept", acceptType);
            using (var response = await _httpClient.GetAsync(url))
            {
                if (!string.IsNullOrWhiteSpace(acceptType))
                    _httpClient.DefaultRequestHeaders.Remove("Accept");
                if (response.StatusCode == HttpStatusCode.BadRequest)
                    throw new ServiceException(response.Content.ReadAsStringAsync().Result.Replace("\"", ""));
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsByteArrayAsync();
            }
        }
        private void RemoveToken(string token)
        {
            AccessTokenItem toRemoveAccessTokenItem;
            var tokenPair = _accessTokens.Where(p => p.Value.AccessToken == token).FirstOrDefault();
            if (tokenPair.Value != null)
            {
                _accessTokens.TryRemove(tokenPair.Key, out toRemoveAccessTokenItem);
                _accessTokenConfiguration.TryRemove(toRemoveAccessTokenItem.AccessToken, out ApiRequestConfiguration conf);
            }
        }
        public async Task<string> InternalServicePostAsync(string url, string input, string contenttype = MediaTypeJson)
        {
            EnsureHttpClientCreated();
            using (var requestContent = new StringContent(input, Encoding.UTF8, contenttype))
            {
                using (var response = await _httpClient.PostAsync(url, requestContent))
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized && response.RequestMessage?.Headers?.Authorization?.Scheme == "Bearer")
                    {
                        RemoveToken(response.RequestMessage?.Headers?.Authorization?.Parameter);
                        throw new ServiceException(ClientSideErrors.AUTHENTICATION_RETRY);
                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                        throw new ServiceException(response.Content.ReadAsStringAsync().Result.Replace("\"", ""));
                    else if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.Accepted)
                        throw new Exception(response.Content.ReadAsStringAsync().Result.Replace("\"", ""));
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }

        public async Task<string> InternalServicePostFileAsync(string url, string filename, byte[] data, object obj, string fileContentType = MediaTypeOctet, string dataContentType = MediaTypeJson)
        {
            EnsureHttpClientCreated();

            using (var requestContent = new MultipartFormDataContent())
            {
                var dataRequestContent = new StringContent(JsonSerializer.Serialize(obj), Encoding.UTF8, dataContentType);
                requestContent.Add(dataRequestContent);

                var dataContent = new ByteArrayContent(data);
                dataContent.Headers.Add("Content-Type", fileContentType);
                dataContent.Headers.Add("Content-Disposition", "form-data; name=\"file\"; filename=\"" + filename + "\"");
                requestContent.Add(dataContent, "file", filename);

                using (var response = await _httpClient.PostAsync(url, requestContent))
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized && response.RequestMessage?.Headers?.Authorization?.Scheme == "Bearer")
                    {
                        RemoveToken(response.RequestMessage?.Headers?.Authorization?.Parameter);
                        throw new ServiceException(ClientSideErrors.AUTHENTICATION_RETRY);
                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                        throw new ServiceException(response.Content.ReadAsStringAsync().Result.Replace("\"", ""));
                    else if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.Accepted)
                        throw new Exception(response.Content.ReadAsStringAsync().Result.Replace("\"", ""));
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }

        public async Task<string> InternalServiceGetAsync(string url)
        {
            _timeout = TimeSpan.FromMinutes(5);
            EnsureHttpClientCreated();
            using (var response = await _httpClient.GetAsync(url))
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized && response.RequestMessage?.Headers?.Authorization?.Scheme == "Bearer")
                {
                    RemoveToken(response.RequestMessage?.Headers?.Authorization?.Parameter);
                    throw new ServiceException(ClientSideErrors.AUTHENTICATION_RETRY);
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new ServiceException(ClientSideErrors.RESOURCE_NOT_FOUND);
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                    throw new ServiceException(response.Content.ReadAsStringAsync().Result.Replace("\"", ""));
                else if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.Accepted)
                    throw new Exception(response.Content.ReadAsStringAsync().Result.Replace("\"", ""));
                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<string> GetAsync(string url)
        {
            EnsureHttpClientCreated();

            using (var response = await _httpClient.GetAsync(url))
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<byte[]> GetByteArrayAsync(string url)
        {
            EnsureHttpClientCreated();

            using (var response = await _httpClient.GetAsync(url))
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsByteArrayAsync();
            }
        }

        public async Task<string> GetAsync(string url, string bodyContent)
        {
            EnsureHttpClientCreated();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url.Contains(_baseUrl) ? url : _baseUrl + url),
                Content = new StringContent(bodyContent, Encoding.UTF8, MediaTypeJson),
            };

            using (var response = await _httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }
        public async Task<string> InternalServiceGetAsync(string url, string bodyContent)
        {
            EnsureHttpClientCreated();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url.Contains(_baseUrl) ? url : _baseUrl + url),
                Content = new StringContent(bodyContent, Encoding.UTF8, MediaTypeJson),
            };

            using (var response = await _httpClient.SendAsync(request))
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized && response.RequestMessage?.Headers?.Authorization?.Scheme == "Bearer")
                {
                    RemoveToken(response.RequestMessage?.Headers?.Authorization?.Parameter);
                    throw new ServiceException(ClientSideErrors.AUTHENTICATION_RETRY);
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new ServiceException(ClientSideErrors.RESOURCE_NOT_FOUND);
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                    throw new ServiceException(response.Content.ReadAsStringAsync().Result.Replace("\"", ""));
                else if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.Accepted)
                    throw new Exception(response.Content.ReadAsStringAsync().Result.Replace("\"", ""));
                return await response.Content.ReadAsStringAsync();
            }
        }
        public async Task<string> PostFileAsync(string url, string filename, byte[] data, string contenttype = MediaTypeOctet)
        {
            EnsureHttpClientCreated();

            using (var requestContent = new MultipartFormDataContent())
            {
                var dataContent = new ByteArrayContent(data);
                dataContent.Headers.Add("Content-Type", contenttype);
                dataContent.Headers.Add("Content-Disposition", "form-data; name=\"file\"; filename=\"" + filename + "\"");
                requestContent.Add(dataContent, "file", filename);
                using (var response = await _httpClient.PostAsync(url, requestContent))
                {
                    if (response.StatusCode == HttpStatusCode.BadRequest)
                        throw new ServiceException(response.Content.ReadAsStringAsync().Result.Replace("\"", ""));
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }
        public async Task<string> PutAsync(string url, object input)
        {
            return await PutAsync(url, new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, MediaTypeJson));
        }

        public async Task<string> PutAsync(string url, HttpContent content)
        {
            EnsureHttpClientCreated();

            using (var response = await _httpClient.PutAsync(url, content))
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }
        public async Task<string> InternalServicePutAsync(string url, object input)
        {
            EnsureHttpClientCreated();
            var content = new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, MediaTypeJson);
            using (var response = await _httpClient.PutAsync(url, content))
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized && response.RequestMessage?.Headers?.Authorization?.Scheme == "Bearer")
                {
                    RemoveToken(response.RequestMessage?.Headers?.Authorization?.Parameter);
                    throw new ServiceException(ClientSideErrors.AUTHENTICATION_RETRY);
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new ServiceException(ClientSideErrors.RESOURCE_NOT_FOUND);
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                    throw new ServiceException(response.Content.ReadAsStringAsync().Result.Replace("\"", ""));
                else if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.Accepted)
                    throw new Exception(response.Content.ReadAsStringAsync().Result.Replace("\"", ""));
                return await response.Content.ReadAsStringAsync();
            }
        }
        public async Task<string> DeleteAsync(string url)
        {
            EnsureHttpClientCreated();

            using (var response = await _httpClient.DeleteAsync(url))
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }
        public async Task<string> DeleteInternalAsync(string url)
        {
            EnsureHttpClientCreated();

            using (var response = await _httpClient.DeleteAsync(url))
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new ServiceException(ClientSideErrors.RESOURCE_NOT_FOUND);
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                    throw new ServiceException(response.Content.ReadAsStringAsync().Result.Replace("\"", ""));
                else if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.Accepted)
                    throw new Exception(response.Content.ReadAsStringAsync().Result.Replace("\"", ""));
                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<string> DeleteInternalAsync(string url, string input)
        {
            EnsureHttpClientCreated();
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(url),
                Content = new StringContent(input, Encoding.UTF8, "application/json")
            };
            using (var response = await _httpClient.SendAsync(request))
            {
                if (response.StatusCode == HttpStatusCode.BadRequest)
                    throw new ServiceException(response.Content.ReadAsStringAsync().Result.Replace("\"", ""));
                else if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.Accepted)
                    throw new Exception(response.Content.ReadAsStringAsync().Result.Replace("\"", ""));
                return await response.Content.ReadAsStringAsync();
            }
        }

        public void Dispose()
        {
            _httpClientHandler?.Dispose();
            _httpClientHandler = null;
            _httpClient?.Dispose();
            _httpClient = null;
        }
        virtual protected void CreateHttpClient()
        {
            _httpClientHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            };
            _httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            _httpClient = new HttpClient(_httpClientHandler, false)
            {
                Timeout = _timeout
            };

            //_httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(ClientUserAgent);

            if (!string.IsNullOrWhiteSpace(_baseUrl))
            {
                _httpClient.BaseAddress = new Uri(_baseUrl);
            }

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeJson));
        }

        #region Access Token
        private void EnsureHttpClientCreated()
        {
            if (_httpClient == null)
            {
                CreateHttpClient();
            }
        }

        public static string GetAccessToken(string identityServerUrl, ApiRequestConfiguration apiSecretConf)
        {
            if (_accessTokens.ContainsKey(apiSecretConf.ApiName))
            {
                var accessToken = _accessTokens.GetValueOrDefault(apiSecretConf.ApiName);
                if (accessToken.ExpiresIn > DateTime.UtcNow)
                {
                    return accessToken.AccessToken;
                }
                else
                {
                    // remove
                    AccessTokenItem toRemoveAccessTokenItem;
                    _accessTokens.TryRemove(apiSecretConf.ApiName, out toRemoveAccessTokenItem);
                    _accessTokenConfiguration.TryRemove(toRemoveAccessTokenItem.AccessToken, out ApiRequestConfiguration conf);
                }
            }

            // add
            var newAccessToken = GetAccessTokenInternal(identityServerUrl, apiSecretConf);
            _accessTokens.TryAdd(apiSecretConf.ApiName, newAccessToken);
            _accessTokenConfiguration.TryAdd(newAccessToken.AccessToken, apiSecretConf);

            return newAccessToken.AccessToken;
        }
        //private static string GetAccessToken(string identityServerUrl, ApiRequestConfiguration apiSecretConf)

        private static AccessTokenItem GetAccessTokenInternal(string identityServerUrl, ApiRequestConfiguration apiSecretConf)
        {
            var client = new HttpClient();
            var disco = client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = identityServerUrl,
                Policy =
                {
                    RequireHttps = false,
                    ValidateIssuerName = false,
                    ValidateEndpoints = false
                }
            }).Result;
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                throw new Exception("Could not get discovery.");
            }

            var tokenResponse = client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = apiSecretConf.Clientid,
                ClientSecret = apiSecretConf.Secret,
                Scope = apiSecretConf.Scope
            }).Result;
            if (tokenResponse.IsError)
                throw new Exception("Could not get access token. Please check identity server configurations.");
            //return tokenResponse.AccessToken;
            return new AccessTokenItem
            {
                ExpiresIn = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
                AccessToken = tokenResponse.AccessToken
            };
        }

        public void SetBearerToken(string accessToken)
        {
            EnsureHttpClientCreated();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        #endregion //Access Token

        public void SetCustomHeader(string header, string val)
        {
            EnsureHttpClientCreated();
            _httpClient.DefaultRequestHeaders.Add(header, val);
        }
        private static string NormalizeBaseUrl(string url)
        {
            return url.EndsWith("/") ? url : url + "/";
        }
    }
}