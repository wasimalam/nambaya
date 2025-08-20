using Common.Interfaces.Interfaces;
using Microsoft.Extensions.Logging;
using RestSharp;
using System.Collections.Generic;
using System.Net;

namespace Common.Interfaces
{
    public class SmsProvider : ISmsProvider
    {
        private readonly SmsSettings _smsSettings;
        private ILogger<SmsProvider> _logger;
        public SmsProvider(SmsSettings smsSettings, ILogger<SmsProvider> logger)
        {
            _smsSettings = smsSettings;
            _logger = logger;
        }

        public string Send(string cellNo, string sender, string messageBody)
        {
            return Send(new List<string> { cellNo }, sender, messageBody);
        }

        public string Send(List<string> cellNumbers, string sender, string messageBody)
        {
            var client = new RestClient(_smsSettings.ClientApiUri);
            client.Timeout = -1;

            var request = new RestRequest(Method.POST);
            request.AddHeader("Accept", _smsSettings.ContentType);
            request.AddHeader("Authorization", "Bearer " + _smsSettings.Token);
            request.AddHeader("Content-Type", _smsSettings.ContentType);

            SmsMessage smsMessage = new SmsMessage
            {
                body = messageBody,
                encoding = _smsSettings.Encoding,
                originator = _smsSettings.SentBy,
                recipients = cellNumbers,
                route = _smsSettings.Route,
                //scheduled_at = DateTime.Now.ToString("yyyy-MM-ddTH:mm:sszzz")
            };

            request.AddJsonBody(smsMessage);

            IRestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                _logger.LogInformation($"SMS sent to {string.Join(",", cellNumbers.ToArray())} successfully with response {response.Content}");
                return response.StatusDescription;
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized) //401
            {
                _logger.LogError($"SMS sent to {string.Join(",", cellNumbers.ToArray())} failed with response {response.StatusCode} : {response.Content}");
                return response.StatusDescription;
            }
            else if (response.StatusCode == HttpStatusCode.NotFound) //404
            {
                _logger.LogError($"SMS sent to {string.Join(",", cellNumbers.ToArray())} failed with response {response.StatusCode} : {response.Content}");
                return response.StatusDescription;
            }
            else if (response.StatusCode == HttpStatusCode.MethodNotAllowed) //405
            {
                _logger.LogError($"SMS sent to {string.Join(",", cellNumbers.ToArray())} failed with response {response.StatusCode} : {response.Content}");
                return response.StatusDescription;
            }
            else if (response.StatusCode == (HttpStatusCode)422) //422
            {
                _logger.LogError($"SMS sent to {string.Join(",", cellNumbers.ToArray())} failed with response {response.StatusCode} : {response.Content}");
                return response.StatusDescription;
            }
            else if (response.StatusCode == (HttpStatusCode)429) //429
            {
                _logger.LogError($"SMS sent to {string.Join(",", cellNumbers.ToArray())} failed with response {response.StatusCode} : {response.Content}");
                return response.StatusDescription;
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError) //500
            {
                _logger.LogError($"SMS sent to {string.Join(",", cellNumbers.ToArray())} failed with response {response.StatusCode} : {response.Content}");
                return response.StatusDescription;
            }
            else if (response.StatusCode == HttpStatusCode.ServiceUnavailable) //503
            {
                _logger.LogError($"SMS sent to {string.Join(",", cellNumbers.ToArray())} failed with response {response.StatusCode} : {response.Content}");
                return response.StatusDescription;
            }
            return response.StatusDescription;
        }
    }

    public class SmsSettings : object
    {
        public string ClientApiUri { get; set; }
        public string ContentType { get; set; }
        public string SentBy { get; set; }
        public string Token { get; set; }
        public string Encoding { get; set; }
        public string Route { get; set; }
    }

    public class SmsMessage
    {
        public string body { get; set; }
        public string encoding { get; set; }
        public string originator { get; set; }
        public List<string> recipients { get; set; }
        public string route { get; set; }
        public string scheduled_at { get; set; }
    }
}