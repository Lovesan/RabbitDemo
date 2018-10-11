using System;
using RabbitDemo.Common;

namespace RabbitDemo.SimpleTracer
{
    public static class Program
    {
        public static void Main()
        {
            Console.Title = "SimpleTracer";
            using (var client = new RabbitClient())
            {
                Console.WriteLine("Press ESC to stop the tracer...");

                // Create an anonymous queue for tracing messages
                var traceQueue = client.DeclareQueue(exclusive: true);

                // Bind the queue to tracing exchange
                client.BindQueue(
                    queue: traceQueue,
                    exchange: "amq.rabbitmq.trace",
                    routingKey: "#");

                // Subscribe to messages available from tracer
                using (client.Subscribe(OnMessage, queue: traceQueue))
                {
                    while (true)
                    {
                        var key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Escape)
                            break;
                    }
                }
            }
        }

        private static void OnMessage(RabbitMessage msg)
        {
            var str = msg.GetBodyAsString();
            Console.WriteLine(
                "MESSAGE {0}\nExchange: {1}\nRouting key: {2}\n{3}\n",
                msg.RoutingKey.StartsWith("publish") ? "PUBLISH" : "DELIVERY",
                msg.Exchange,
                msg.RoutingKey,
                str);
        }
    }
}
