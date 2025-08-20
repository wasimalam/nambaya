using MassTransit;
using Patient.Contracts.Models;
using Pharmacist.Contracts.Interfaces;
using System.Threading.Tasks;

namespace Pharmacist.API.Consumers
{
    public class DeviceReminderConsumer : IConsumer<DeviceReminderPayLoadBO>
    {
        private IDeviceReminderService _deviceReminderService;
        public DeviceReminderConsumer(IDeviceReminderService deviceReminderService)
        {
            _deviceReminderService = deviceReminderService;
        }
        /// <summary>
        /// When a user is added in the database, an event is fired
        /// This function consume the event
        /// It sends an email to the newly created user to say welcome
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Consume(ConsumeContext<DeviceReminderPayLoadBO> context)
        {
            await Task.Run(() => _deviceReminderService.RemindOfDevice(context.Message));
        }
    }
}
