using MassTransit;
using Patient.Contracts.Models;
using Pharmacist.Contracts.Interfaces;
using System.Threading.Tasks;

namespace Pharmacist.API.Consumers
{
    internal class QuickEvaluationNotifier : IConsumer<PatientBO>
    {
        private IPharmacistNotificationService _service;
        public QuickEvaluationNotifier(IPharmacistNotificationService service)
        {
            _service = service;
        }
        public async Task Consume(ConsumeContext<PatientBO> context)
        {
            await Task.Run(() => _service.NotifyQuickEvaluation(context.Message));
        }
    }
}