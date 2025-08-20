using CentralGroup.Contracts.Interfaces;
using MassTransit;
using Patient.Contracts.Models;
using System.Threading.Tasks;

namespace CentralGroup.API.Consumers
{
    internal class QEResultEventConsumer : IConsumer<QEResultEventPayloadBO>
    {
        private IQEResultEventNotificationService _service;
        public QEResultEventConsumer(IQEResultEventNotificationService service)
        {
            _service = service;
        }
        public async Task Consume(ConsumeContext<QEResultEventPayloadBO> context)
        {
            await Task.Run(() => _service.Notify(context.Message));
        }
    }
}