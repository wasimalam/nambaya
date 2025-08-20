using Common.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.IO;
using System.Threading;

namespace Logging.API.RabbitMQ
{
    public class RabbitMQPersistentConnection : IRabbitMQPersistentConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private EventBusRabbitMQ _eventBusRabbitMQ;
        private IConnection _connection;
        private bool _disposed;
        private IServiceProvider _serviceProvider;
        public RabbitMQPersistentConnection(IConnectionFactory connectionFactory, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            if (!IsConnected)
            {
                TryConnect();
            }
        }

        public void CreateConsumerChannel()
        {
            if (!IsConnected)
            {
                TryConnect();
            }
            //CHannel 01
            _eventBusRabbitMQ = _serviceProvider.GetRequiredService<EventBusRabbitMQ>(); //new EventBusRabbitMQ(this, "serilog-sink-queue");
            _eventBusRabbitMQ.CreateConsumerChannel();

        }

        public void Disconnect()
        {
            if (_disposed)
            {
                return;
            }
            Dispose();
        }


        public bool IsConnected
        {
            get
            {
                return _connection != null && _connection.IsOpen && !_disposed;
            }
        }

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }
            return _connection.CreateModel();
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            try
            {
                _connection.Dispose();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public bool TryConnect()
        {

            var settings = _serviceProvider.GetRequiredService<RabbitMQSettings>();
            try
            {
                Console.WriteLine("RabbitMQ Client is trying to connect");
                _connection = _connectionFactory.CreateConnection(settings.ConnectionName);
            }
            catch (BrokerUnreachableException e)
            {
                Console.WriteLine(e.Message);
                Thread.Sleep(5000);
                Console.WriteLine("RabbitMQ Client is trying to reconnect");
                _connection = _connectionFactory.CreateConnection(settings.ConnectionName);
            }

            if (IsConnected)
            {
                _connection.ConnectionShutdown += OnConnectionShutdown;
                _connection.CallbackException += OnCallbackException;
                _connection.ConnectionBlocked += OnConnectionBlocked;

                Console.WriteLine($"RabbitMQ persistent connection acquired a connection {_connection.Endpoint.HostName} and is subscribed to failure events");

                return true;
            }
            else
            {
                //  implement send warning email here
                //-----------------------
                Console.WriteLine("FATAL ERROR: RabbitMQ connections could not be created and opened");
                return false;
            }

        }

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (_disposed) return;
            Console.WriteLine("A RabbitMQ connection is shutdown. Trying to re-connect...");
            TryConnect();
        }

        private void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (_disposed) return;
            Console.WriteLine("A RabbitMQ connection throw exception. Trying to re-connect...");
            TryConnect();
        }

        private void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (_disposed) return;
            Console.WriteLine("A RabbitMQ connection is on shutdown. Trying to re-connect...");
            TryConnect();
        }
    }
}
