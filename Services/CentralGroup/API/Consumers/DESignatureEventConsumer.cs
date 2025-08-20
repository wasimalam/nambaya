using CentralGroup.Contracts.Interfaces;
using MassTransit;
using Patient.Contracts.Models;
using System.Threading.Tasks;

namespace CentralGroup.API.Consumers
{
    internal class DESignatureEventConsumer : IConsumer<PatientUserOtp>
    {
        private IDESignatureEventService _service;
        public DESignatureEventConsumer(IDESignatureEventService service)
        {
            _service = service;
        }
        public async Task Consume(ConsumeContext<PatientUserOtp> context)
        {
            await Task.Run(() => _service.DESignatureOTPNotify(context.Message));
        }
    }
}