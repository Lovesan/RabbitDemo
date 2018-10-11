using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitDemo.Common
{
    /// <summary>
    /// RabbitMQ message broker client
    /// </summary>
    public class RabbitClient : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _model;
        private bool _disposed;

        /// <summary>
        /// Constructs new RabbitMQ client
        /// </summary>
        /// <param name="enableQos">Enable Qos to handle long-running message processing</param>
        /// <param name="host">RMQ host name</param>
        /// <param name="user">RMQ user</param>
        /// <param name="password">RMQ password</param>
        public RabbitClient(
            bool enableQos = false,
            string host = "localhost",
            string user = "guest",
            string password = "guest")
        {
            var factory = new ConnectionFactory
            {
                HostName = host,
                UserName = user,
                Password = password
            };
            _connection = factory.CreateConnection();
            _model = _connection.CreateModel();
            if (enableQos)
            {
                _model.BasicQos(0, 1, false);
            }
        }

        public string DeclareQueue(string name = "", bool durable = false, bool exclusive = false, bool autoDelete = true)
        {
            return _model.QueueDeclare(
                queue: name,
                durable: durable,
                exclusive: exclusive,
                autoDelete: autoDelete).QueueName;
        }

        public void DeclareExchange(string name, string type = "direct", bool durable = false, bool autoDelete = false, bool delayed = false)
        {
            var args = new Dictionary<string, object>();
            if (delayed)
            {
                args.Add("x-delayed-type", type);
                type = "x-delayed-message";
            }
            _model.ExchangeDeclare(
                exchange: name,
                type: type,
                durable: durable,
                autoDelete: autoDelete,
                arguments: args);
        }

        public void BindQueue(string queue, string exchange, string routingKey)
        {
            _model.QueueBind(
                queue: queue,
                exchange: exchange,
                routingKey: routingKey);
        }

        public void Publish<T>(
            T obj,
            string routingKey = "",
            string exchange = "",
            bool persistent = false,
            int delayMs = 0,
            string replyTo = null,
            string correlationId = null)
        {
            var props = _model.CreateBasicProperties();
            props.Persistent = persistent;
            if (!string.IsNullOrWhiteSpace(replyTo))
            {
                props.ReplyTo = replyTo;
            }

            if (!string.IsNullOrWhiteSpace(correlationId))
            {
                props.CorrelationId = correlationId;
            }
            if (delayMs > 0)
            {
                props.Headers = new Dictionary<string, object>
                {
                    { "x-delay", delayMs }
                };
            }
            var data = SerializerHelper.Write(obj);
            _model.BasicPublish(
                exchange: exchange,
                routingKey: routingKey,
                basicProperties: props,
                body: data);
        }

        public IDisposable Subscribe(Action<RabbitMessage> callback, string queue, bool autoAck = true)
        {
            var consumer = new EventingBasicConsumer(_model);
            consumer.Received += (model, args) => callback(new RabbitMessage(this, _model, args, autoAck));
            var tag = _model.BasicConsume(
                queue: queue,
                autoAck: autoAck,
                exclusive: false,
                consumer: consumer);
            return new RabbitSubscribition(_model, tag);
        }

        public IDisposable SubscribeAsync(Func<RabbitMessage, Task> callback, string queue, bool autoAck = true)
        {
            var consumer = new AsyncEventingBasicConsumer(_model);
            consumer.Received += (model, args) => callback(new RabbitMessage(this, _model, args, autoAck));
            var tag = _model.BasicConsume(
                queue: queue,
                autoAck: autoAck,
                exclusive: false,
                consumer: consumer);
            return new RabbitSubscribition(_model, tag);
        }

        /// <summary>
        /// Disposes RMQ client
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _model?.Dispose();
                _connection?.Dispose();
            }
        }

        ~RabbitClient()
        {
            Dispose();
        }
    }
}
