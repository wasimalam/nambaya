using System.Threading.Tasks;
using Common.BusinessObjects.ConsumerMessages;
using MassTransit;
using Patient.Contracts.Interfaces;

namespace Patient.API.Consumers
{
    public class PatientCaseStatusConsumer : IConsumer<PatientCaseStatusDetailMessage>
    {
        private readonly ICaseDispatchService _caseDispatchService;
        public PatientCaseStatusConsumer(ICaseDispatchService caseDispatchService)
        {
            _caseDispatchService = caseDispatchService;
        }
        public Task Consume(ConsumeContext<PatientCaseStatusDetailMessage> context)
        {
            _caseDispatchService.UpdateCaseDetail(context.Message);
            return Task.CompletedTask;
        }
    }
}
