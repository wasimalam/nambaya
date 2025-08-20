using MassTransit;
using MassTransit.RabbitMqTransport.Scheduling;
using MassTransit.RabbitMqTransport.Topology;
using MassTransit.Scheduling;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Common.Infrastructure
{
    public class RabbitMQClient
    {
        private readonly RabbitMQSettings _rabbitMQSettings;
        private readonly IBusControl _bus;
        private readonly IServiceProvider _serviceProvider;
        public RabbitMQClient(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _bus = serviceProvider.GetRequiredService<IBusControl>();
            _rabbitMQSettings = serviceProvider.GetRequiredService<RabbitMQSettings>();
        }
        public void SendMessage(string channel, object obj)
        {
            if (_rabbitMQSettings.Port == 0)
                _rabbitMQSettings.Port = 5672;
            Uri uri = new Uri($"rabbitmq://{_rabbitMQSettings.Host}:{_rabbitMQSettings.Port}/{channel}");
            var endPoint = _bus.GetSendEndpoint(uri).Result;
            endPoint.Send(obj);
        }
        public void ScheduleSendMessage<T>(string channel, T obj, TimeSpan delay) where T : class
        {
            if (_rabbitMQSettings.Port == 0)
                _rabbitMQSettings.Port = 5672;
            var ms = new MessageScheduler(new DelayedExchangeScheduleMessageProvider(_bus, _bus.Topology as IRabbitMqHostTopology));
            ms.ScheduleSend(new Uri($"rabbitmq://{_rabbitMQSettings.Host}:{_rabbitMQSettings.Port}/{channel}"), delay, obj);
        }
    }

    public class KnownChannels
    {
        public const string EMAIL_CHANNEL = "send_email";
        public const string SMS_CHANNEL = "send_sms";
        public const string QUICK_EVALUATION_COMPLETED_EVENT_CHANNEL = "quick_evaluation_completed_event_channel";
        public const string QUICK_EVALUATION_RESULT_EVENT_CHANNEL = "quick_evaluation_result_event_channel";
        public const string DETAILED_EVALUATION_UPLOADED_COMPLETED_EVENT_CHANNEL = "detailed_evaluation_uploaded_completed_event_channel";
        public const string CASE_DISPATCH_EVENT_EVENT_CHANNEL = "case_dispatch_event_event_channel";
        public const string NAVIGATOR_CREATE_CARDIOLOGIST = "navigator_create_cardiologist";
        public const string NAVIGATOR_UPDATE_CARDIOLOGIST = "navigator_update_cardiologist";
        public const string NAVIGATOR_EDF_FILE_UPLOADED_EVENT_CHANNEL = "navigator_edf_file_uploaded_event_channel";
        public const string USER_REGISTER_EVENT_CHANNEL = "user_register_event_channel";
        public const string PHONE_VERIFICATION_EVENT_CHANNEL = "phone_verification_event_channel";
        public const string QUICK_EVALUATION_PROGRESS_CHANNEL = "quick_evaluation_progress_channel";
        public const string DEVICE_REMINDER_EVENT_CHANNEL = "device_reminder_event_channel";
        public const string SIGNATURE_SAVE_EVENT_CHANNEL = "signature_save_event_channel";
        public const string SIGNATURE_DELETE_EVENT_CHANNEL = "signature_delete_event_channel";
        public const string DETAIL_REPORT_SIGN_EVENT_CHANNEL = "detail_report_sign_event_channel";
        public const string FILE_SYNC_EVENT_CHANNEL = "file_sync_event_channel";
        public const string PATIENT_DEACTIVATE_VERIFICATION_CHANNEL = "patient_deactivate_verification_channel";
        public const string PATIENT_AEGNORD_CHANNEL = "patient_aegnord";
        public const string AEGNORD_PATIENT_CHANNEL = "aegnord_patient";
    }
}
