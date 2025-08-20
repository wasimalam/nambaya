using Common.Infrastructure;
using Logging.Contracts.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog.Formatting.Compact.Reader;
using System;
using System.Text;

namespace Logging.API.RabbitMQ
{
    public class EventBusRabbitMQ : IDisposable
    {
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private IModel _consumerChannel;
        private string _queueName;
        private ILoggingInsertService _consumerService;
        private IServiceProvider _serviceProvider;
        public EventBusRabbitMQ(IServiceProvider serviceProvider, IRabbitMQPersistentConnection persistentConnection)
        {
            _serviceProvider = serviceProvider;
            _consumerService = serviceProvider.GetRequiredService<ILoggingInsertService>();
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _queueName = SerilogRabbitMQ.Queue;
        }

        public IModel CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var channel = _persistentConnection.CreateModel();
            channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += ReceivedEvent;

            channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
            channel.CallbackException += (sender, ea) =>
            {
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
            };
            return channel;
        }

        private void ReceivedEvent(object sender, BasicDeliverEventArgs e)
        {
            if (e.RoutingKey == SerilogRabbitMQ.Routing)
            {
                var message = Encoding.UTF8.GetString(e.Body);
                var evt = LogEventReader.ReadFromString(message);
                if (evt != null)
                {
                    _consumerService.InsertRecord(evt);
                }
            }
        }
        public void Dispose()
        {
            if (_consumerChannel != null)
            {
                _consumerChannel.Dispose();
            }
        }
    }
}
