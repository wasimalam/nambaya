using Common.BusinessObjects;
using Common.Infrastructure;
using MassTransit;
using NambayaUser.Contracts.Interfaces;
using System.Threading.Tasks;

namespace NambayaUser.API
{
    internal class PhoneVerifcationNotifier : IConsumer<UserOtp>
    {
        private IPhoneVerificationNotificationService _service;
        public PhoneVerifcationNotifier(IPhoneVerificationNotificationService service)
        {
            _service = service;
        }
        public async Task Consume(ConsumeContext<UserOtp> context)
        {
            await Task.Run(() => _service.NotifyPhoneVerification(context.Message));
        }
    }
}