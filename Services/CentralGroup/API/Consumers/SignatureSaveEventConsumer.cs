using CentralGroup.Contracts.Interfaces;
using Common.Infrastructure;
using MassTransit;
using System.Threading.Tasks;

namespace CentralGroup.API.Consumers
{
    internal class SignatureSaveEventConsumer : IConsumer<UserOtp>
    {
        private ISignatureSaveEventService _service;
        public SignatureSaveEventConsumer(ISignatureSaveEventService service)
        {
            _service = service;
        }
        public async Task Consume(ConsumeContext<UserOtp> context)
        {
            await Task.Run(() => _service.SignatureSaveOTPNotify(context.Message));
        }
    }
}