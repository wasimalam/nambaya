using RabbitMQ.Client;
using System;

namespace Logging.API.RabbitMQ
{
    public interface IRabbitMQPersistentConnection : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();

        void CreateConsumerChannel();

        void Disconnect();
    }
}
