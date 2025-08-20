using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Common.Services.Extensions;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Services.Handlers
{
    public class RequestHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestHandler(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var correlationid = GetCorrelationId();
            var session = GetSessionContext();
            if (string.IsNullOrWhiteSpace(correlationid) == false)
                request.Headers.Add(LoggingConstants.CorrelationId, GetCorrelationId()); // Getting correlationid from request context. 
            if (session != null && request.Headers.Contains("NambayaSession") == false)
                request.Headers.Add("NambayaSession", JsonSerializer.Serialize(session));
            return base.SendAsync(request, cancellationToken);
        }
        private string GetCorrelationId()
        {
            try
            {
                var context = this._httpContextAccessor.HttpContext;
                if (context != null)
                {
                    var result = context?.Items[LoggingConstants.CorrelationId] as string;
                    return result;
                }

                return Thread.GetData(Thread.GetNamedDataSlot(LoggingConstants.CorrelationId)) as string;
            }
            catch //(Exception exception)
            {
                throw new ServiceException("Unable to get CorrelationId id in header");
            }
        }
        private object GetSessionContext()
        {
            object obj = this._httpContextAccessor.HttpContext?.GetSessionContext();
            if (obj == null)
                obj = Thread.GetData(Thread.GetNamedDataSlot("NambayaSession"));
            return obj;
        }
    }
}
