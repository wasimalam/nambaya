using Common.BusinessObjects;
using MassTransit;
using NambayaUser.Contracts.Interfaces;
using System.Threading.Tasks;

namespace NambayaUser.API
{
    internal class UserRegistrationNotifier : IConsumer<BaseUserBO>
    {
        private IUserRegistrationNotificationService _service;
        public UserRegistrationNotifier(IUserRegistrationNotificationService service)
        {
            _service = service;
        }
        public async Task Consume(ConsumeContext<BaseUserBO> context)
        {
            await Task.Run(() => _service.NotifyRegitration(context.Message));
        }
    }
}