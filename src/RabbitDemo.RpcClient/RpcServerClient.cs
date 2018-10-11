using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using RabbitDemo.Common;

namespace RabbitDemo.RpcClient
{
    /// <summary>
    /// RPC client implementation
    /// </summary>
    public class RpcServerClient : IDisposable
    {
        private const int TimeoutMs = 10000;

        private readonly RabbitClient _client;
        private readonly string _responseQueue;
        private readonly ConcurrentDictionary<string, WorkItem> _requests;
        private bool _disposed;

        /// <summary>
        /// Constructs new <see cref="RpcServerClient"/>
        /// </summary>
        public RpcServerClient()
        {
            _requests = new ConcurrentDictionary<string, WorkItem>();
            _client = new RabbitClient(enableQos: true);

            // Declare request queue
            _client.DeclareQueue("rd_rpc_queue");

            // Declare response queue
            _responseQueue = _client.DeclareQueue(exclusive: true);

            // Subscribe to responses
            _client.Subscribe(OnMessage, _responseQueue);
        }
        
        /// <summary>
        /// Makes a call to remote RPC server
        /// </summary>
        /// <param name="value">Parameter</param>
        /// <returns>Processed value</returns>
        public Task<string> Call(string value)
        {
            var correlationId = Guid.NewGuid().ToString();

            // Create the request item and remember it
            var tcs = new TaskCompletionSource<string>();
            var cancel = new CancellationTokenSource();
            var workItem = new WorkItem
            {
                CompletionSource = tcs,
                TimeoutCancellationSource = cancel
            };
            _requests[correlationId] = workItem;

            // Schedule timeout event
            Task.Delay(TimeoutMs, cancel.Token)
                .ContinueWith(OnTimeout, correlationId);

            // Publish the request
            _client.Publish(
                value,
                routingKey: "rd_rpc_queue",
                exchange: "",
                replyTo: _responseQueue,
                correlationId: correlationId);

            return tcs.Task;
        }

        /// <summary>
        /// Handle timeout event
        /// </summary>
        /// <param name="_">Previous (delay) task</param>
        /// <param name="state">CorrelationId parameter</param>
        private void OnTimeout(Task _, object state)
        {
            var correlationId = (string) state;
            if (_requests.TryRemove(correlationId, out var wi))
            {
                wi.CompletionSource.SetException(new TimeoutException());
            }
        }

        /// <summary>
        /// Handle message from response queue
        /// </summary>
        /// <param name="msg">Response message</param>
        private void OnMessage(RabbitMessage msg)
        {
            if (!_requests.TryRemove(msg.CorrelationId, out var wi))
                return;

            // Cancel timeout task
            wi.TimeoutCancellationSource.Cancel();

            // Complete the task
            var result = msg.Read<string>();
            wi.CompletionSource.SetResult(result);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _client.Dispose();
            }
        }

        private class WorkItem
        {
            public TaskCompletionSource<string> CompletionSource { get; set; }

            public CancellationTokenSource TimeoutCancellationSource { get; set; }
        }
    }
}
