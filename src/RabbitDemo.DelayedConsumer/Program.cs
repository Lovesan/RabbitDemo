using RabbitDemo.Common;
using System;

namespace RabbitDemo.DelayedConsumer
{
    public static class Program
    {
        public static void Main()
        {
            Console.Title = "DelayedConsumer";
            using (var client = new RabbitClient())
            {
                // Declare the exchange
                client.DeclareExchange(
                    "rd.delayed", // exchange name
                    autoDelete: true,
                    delayed: true);

                // Declare anonymous queue
                var queue = client.DeclareQueue(exclusive: true);

                // Bind queue to the exchange
                client.BindQueue(queue, "rd.delayed", "");

                Console.WriteLine("Press ESC to exit...");

                // Subscribe to messages available from the queue
                using (client.Subscribe(OnMessage, queue: queue))
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
            var str = msg.Read<string>();
            Console.WriteLine("Message received: {0}", str);
        }
    }
}
