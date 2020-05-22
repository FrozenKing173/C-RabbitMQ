using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ContactDetailsServiceB.BusinessModels;

namespace ContactDetailsServiceB.DataAccessLayer.ServiceBus.RPC_ContactDetails
{
    public class RpcServer_ContactDetails : RpcBase
    {
        public string Response { get; private set; }
        ContactDetailsModel _contactDetailsModel = null;
        public event EventHandler OnDataChange;

        public RpcServer_ContactDetails() : base()
        {
            //Declare Channel exists, durable
            _channel.QueueDeclare("ContactDetails-queue", true, false, false);

            //Even messageing
            _channel.BasicQos(0, 1, false);

            //Retrieve BasicDeliverEventArgs without auto acknowleding 
            _channel.BasicConsume(queue: "ContactDetails-queue",
                                     autoAck: false,
                                     consumer: _consumer);
            _consumer.Received += Delivery;
        }

        protected override void Delivery(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body;
            var props = ea.BasicProperties;

            // Properties for replying to the same Producer/publisher
            _basicProperties = _channel.CreateBasicProperties();
            _basicProperties.CorrelationId = props.CorrelationId;
            _basicProperties.Persistent = true;
            _basicProperties.ReplyTo = props.ReplyTo;

            // Determines if Name is valid
            bool success = false; 
            try
            {
                // Validate Name string
                _contactDetailsModel = new ContactDetailsModel();
                Response = Encoding.UTF8.GetString(body);
                OnDataChange(this, new EventArgs());
                success = _contactDetailsModel.TrySetName(Response);

                if (success)
                {
                    var message = "Hello " + Encoding.UTF8.GetString(body);
                    message += ", I am your father!";
                    Response = message;

                    OnDataChange(this, new EventArgs());
                }
                else
                {
                    Response = Encoding.UTF8.GetString(body) + " is not valid.";

                    OnDataChange(this, new EventArgs());
                }

                Log("RPCServer", 1, "Received: " + Encoding.UTF8.GetString(body));

            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("-----");
                Console.WriteLine("RPC Server: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("-----");
            }
            finally
            {
                
                Console.ForegroundColor = ConsoleColor.White;
                Log("RPCServer", 1, "Sending: " + Response);
                Send(Response);

                //Acknowledge retrieving consumer from queue
                _channel.BasicAck(deliveryTag: ea.DeliveryTag,
                 multiple: false);
            }
        }

        public override void Send(string response)
        {
            // Basic publish replying to producer/publisher from this consumer.
            var responseBytes = Encoding.UTF8.GetBytes(response);
            _channel.BasicPublish(exchange: "",
                   routingKey: _basicProperties.ReplyTo,
                   basicProperties: _basicProperties,
                   body: responseBytes);

        }

        public override string ToString()
        {
            return "RPCServer successfully booted!";
        }
    }
}
