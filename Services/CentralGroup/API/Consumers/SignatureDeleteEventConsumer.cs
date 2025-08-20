using CentralGroup.Contracts.Interfaces;
using Common.Infrastructure;
using MassTransit;
using System.Threading.Tasks;

namespace CentralGroup.API.Consumers
{
    internal class SignatureDeleteEventConsumer : IConsumer<UserOtp>
    {
        private ISignatureDeleteEventService _service;
        public SignatureDeleteEventConsumer(ISignatureDeleteEventService service)
        {
            _service = service;
        }
        public async Task Consume(ConsumeContext<UserOtp> context)
        {
            await Task.Run(() => _service.SignatureDeleteOTPNotify(context.Message));
        }
    }
}