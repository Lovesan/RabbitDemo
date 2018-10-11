using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitDemo.Common
{
    /// <summary>
    /// Helper class for handling messages on consumer side
    /// </summary>
    public class RabbitMessage
    {
        private readonly BasicDeliverEventArgs _ea;
        private readonly IModel _model;
        private bool _handled;

        /// <summary>
        /// Constructs new <see cref="RabbitMessage"/>
        /// </summary>
        /// <param name="client">RabbitMQ client</param>
        /// <param name="model">RabbitMQ channel</param>
        /// <param name="ea">RabbitMQ message parameters</param>
        /// <param name="autoAck">Whether the message is handled automatically</param>
        internal RabbitMessage(RabbitClient client, IModel model, BasicDeliverEventArgs ea, bool autoAck)
        {
            Client = client;
            _ea = ea;
            _model = model;
            _handled = autoAck;
        }

        /// <summary>
        /// RabbitMQ client
        /// </summary>
        public RabbitClient Client { get; }

        /// <summary>
        /// An exchange the message have been published to
        /// </summary>
        public string Exchange => _ea.Exchange;

        /// <summary>
        /// Routing key of the message
        /// </summary>
        public string RoutingKey => _ea.RoutingKey;

        /// <summary>
        /// Reply queue name
        /// </summary>
        public string ReplyTo => _ea.BasicProperties.ReplyTo;

        /// <summary>
        /// Response correlation id
        /// </summary>
        public string CorrelationId => _ea.BasicProperties.CorrelationId;

        /// <summary>
        /// Acknowledges the message
        /// </summary>
        public void Ack()
        {
            if (!_handled)
            {
                _model.BasicAck(_ea.DeliveryTag, false);
                _handled = true;
            }
        }

        /// <summary>
        /// Rejects the delivered message
        /// </summary>
        /// <param name="requeue">Whether to requeue message</param>
        public void Reject(bool requeue = true)
        {
            if (!_handled)
            {
                _model.BasicReject(_ea.DeliveryTag, requeue);
                _handled = true;
            }
        }

        /// <summary>
        /// Gets the string representation of body bytes
        /// </summary>
        /// <returns>Body as string</returns>
        public string GetBodyAsString()
        {
            var body = _ea.Body;
            return Encoding.UTF8.GetString(body);
        }

        /// <summary>
        /// Deserializes an object from the message
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <returns>Deserialized object</returns>
        public T Read<T>()
        {
            var body = _ea.Body;
            return SerializerHelper.Read<T>(body, 0, body.Length);
        }
    }
}
