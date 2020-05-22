using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ContactDetailsServiceA.DataAccessLayer.ServiceBus.RPC_ContactDetails
{
    public class RpcClient_ContactDetails : RpcBase
    {
        private readonly string _replyBackQueueName = "";
        private readonly BlockingCollection<string> _respQueue = null;

        public event EventHandler OnDataChange;

        public RpcClient_ContactDetails() : base()
        {
            _respQueue = new BlockingCollection<string>();

            // Declare the channel, exchange and queue durable.
            _channel.ExchangeDeclare("ContactDetails-exchange", ExchangeType.Direct);
            _channel.QueueDeclare("ContactDetails-queue", true, false, false);

           
            // Set reply Back QueueName
            _replyBackQueueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind("ContactDetails-queue", "ContactDetails-exchange", _replyBackQueueName, null);
            

            // Channel properties
            var correlationId = Guid.NewGuid().ToString();
            _basicProperties = _channel.CreateBasicProperties();

            // Is used for receiving replies.
            _basicProperties.CorrelationId = correlationId;

            // Subscribing callback queue
            _basicProperties.ReplyTo = _replyBackQueueName;
            _basicProperties.Persistent = true;

            // Event handler for when the RpcServer replies back here, through the replyBackQueueName
            // Subscribe Delivery to RabbitMQ.Client.Events.BasicDeliverEventArgs
            _consumer.Received += Delivery;



        }
        protected override void Delivery(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body;
            var response = Encoding.UTF8.GetString(body);
            if (ea.BasicProperties.CorrelationId == _basicProperties.CorrelationId)
            {
                _respQueue.Add(response);
                Log("RPCClient", 1, "Received: " + response);
                OnDataChange(this, new EventArgs()); //Notify 
            }
        }
        public override void Send(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);

            lock (_channel)
            {
                // Basic Publish to the ContactDetails-exchange
                _channel.BasicPublish(
                    exchange: "ContactDetails-exchange",
                    routingKey: _basicProperties.ReplyTo,
                    basicProperties: _basicProperties,
                    body: messageBytes);

                // Basic Consume acknowledgement
                _channel.BasicConsume(
                    consumer: _consumer,
                    queue: _replyBackQueueName,
                    autoAck: true);


                Log("RPCClient", 1, "Sending:  " + message);
            }
        }

        public string GetResponse()
        {
            return _respQueue.Take();
        }
        public override string ToString()
        {
            return "RPCClient successfully booted!";
        }
    }
}
