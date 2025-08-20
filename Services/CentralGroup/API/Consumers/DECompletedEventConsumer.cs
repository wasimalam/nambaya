using CentralGroup.Contracts.Interfaces;
using MassTransit;
using Patient.Contracts.Models;
using System.Threading.Tasks;

namespace CentralGroup.API.Consumers
{
    internal class DECompletedEventConsumer : IConsumer<DECompletedEventPayloadBO>
    {
        private IDECompletedEventNotificationService _service;
        public DECompletedEventConsumer(IDECompletedEventNotificationService service)
        {
            _service = service;
        }
        public async Task Consume(ConsumeContext<DECompletedEventPayloadBO> context)
        {
            await Task.Run(() => _service.Notify(context.Message));
        }
    }
}