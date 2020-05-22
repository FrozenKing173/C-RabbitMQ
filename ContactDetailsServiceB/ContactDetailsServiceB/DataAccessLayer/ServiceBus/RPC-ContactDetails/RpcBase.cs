using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using LoggerTool;

namespace ContactDetailsServiceB.DataAccessLayer.ServiceBus.RPC_ContactDetails
{
    public class RpcBase : ILogger
    {
        private Logger _logger = null;
        protected readonly IConnection _connection;
        protected readonly IModel _channel;
        protected readonly EventingBasicConsumer _consumer;
        protected IBasicProperties _basicProperties;
        private static readonly object _writerLock = new object();

        public RpcBase()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            factory.AutomaticRecoveryEnabled = true;
            factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);

            _connection = factory.CreateConnection();

            _channel = _connection.CreateModel();
            _consumer = new EventingBasicConsumer(_channel);
        }
        public virtual void Send(string input) { }


        public void Close()
        {
            _connection.Close();
        }
        public override string ToString()
        {
            return "RPC Base initialized!";
        }
        protected virtual void Delivery(object model, BasicDeliverEventArgs ea) { }

        // ILogger
        public void Log(string fileName, int severity, string message)
        {
            _logger = Logger.getInstance();

            lock (_writerLock)
            {
                _logger.WriteToFile(fileName, severity, message);
            }
        }
    }
}
