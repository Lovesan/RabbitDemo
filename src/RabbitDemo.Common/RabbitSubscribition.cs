using System;
using RabbitMQ.Client;

namespace RabbitDemo.Common
{
    /// <summary>
    /// Represents RabbitMQ message subscription
    /// </summary>
    internal class RabbitSubscribition : IDisposable
    {
        private readonly IModel _model;
        private readonly string _consumerTag;
        private bool _disposed;

        /// <summary>
        /// Constructs new <see cref="RabbitSubscribition"/>
        /// </summary>
        /// <param name="model">RabbitMQ channel</param>
        /// <param name="consumerTag">Consumer tag</param>
        public RabbitSubscribition(IModel model, string consumerTag)
        {
            _model = model;
            _consumerTag = consumerTag;
        }

        /// <summary>
        /// Releases the subscription
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _model.BasicCancel(_consumerTag);
            }
        }
    }
}
