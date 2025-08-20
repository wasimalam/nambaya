using Common.Infrastructure;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Patient.API.Hubs;
using System.Threading.Tasks;

namespace Patient.API.Consumers
{
    public class QuickEvalProgressConsumer : IConsumer<QuickEvaluationProgressMessage>
    {
        public IHubContext<QuickEvalProgressHub> _hubContext { get; private set; }
        public QuickEvalProgressConsumer(IHubContext<QuickEvalProgressHub> hubContext)
        {
            _hubContext = hubContext;
        }
        /// <summary>
        /// When a user is added in the database, an event is fired
        /// This function consume the event
        /// It sends an email to the newly created user to say welcome
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Consume(ConsumeContext<QuickEvaluationProgressMessage> context)
        {
            if (QuickEvalProgressHub.ConnectionCaseMap != null && QuickEvalProgressHub.ConnectionCaseMap.ContainsKey(context.Message.PatientCaseId.ToString()))
                foreach(var clientid in QuickEvalProgressHub.ConnectionCaseMap[context.Message.PatientCaseId.ToString()])
                await this._hubContext.Clients.Client(clientid).SendAsync("receiveprogress", context.Message);
        }
    }
}
