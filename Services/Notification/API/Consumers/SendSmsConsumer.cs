using Common.BusinessObjects;
using Common.Infrastructure;
using Common.Interfaces;
using Common.Interfaces.Interfaces;
using MassTransit;
using System.Threading.Tasks;

namespace Notification.API.Consumers
{
    public class SendSmsConsumer : IConsumer<NotificationBO>
    {
        public ISmsProvider SmsService { get; private set; }
        private readonly ILoggingService _loggingService;
        public SendSmsConsumer(SmsProvider smsService, ILoggingService loggingService)
        {
            SmsService = smsService;
            _loggingService = loggingService;
        }


        /// <summary>
        /// When a user is added in the database, an event is fired
        /// This function consume the event
        /// It sends an email to the newly created user to say welcome
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Consume(ConsumeContext<NotificationBO> context)
        {
            if (context.Message.Addresses == null)
                await Task.Run(() =>
                {
                    SmsService.Send(context.Message.Address, context.Message.SentBy, context.Message.Body);
                    if (context.Message.LogIt)
                        _loggingService.LogNotification(context.Message, NotificationTemplateType.SMS);
                });
            else
                await Task.Run(() =>
                {
                    SmsService.Send(context.Message.Addresses, context.Message.SentBy, context.Message.Body);
                    if (context.Message.LogIt)
                        _loggingService.LogNotification(context.Message, NotificationTemplateType.SMS);
                });
        }
    }
}
