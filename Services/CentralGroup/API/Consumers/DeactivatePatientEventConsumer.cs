using CentralGroup.Contracts.Interfaces;
using MassTransit;
using Patient.Contracts.Models;
using System.Threading.Tasks;

namespace CentralGroup.API.Consumers
{
    internal class DeactivatePatientEventConsumer : IConsumer<PatientUserOtp>
    {
        private IDeactivatePatientEventService _service;
        public DeactivatePatientEventConsumer(IDeactivatePatientEventService service)
        {
            _service = service;
        }
        public async Task Consume(ConsumeContext<PatientUserOtp> context)
        {
            await Task.Run(() => _service.DeactivatePatienOTPNotify(context.Message));
        }
    }
}