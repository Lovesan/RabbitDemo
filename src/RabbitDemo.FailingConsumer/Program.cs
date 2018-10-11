using RabbitDemo.Common;
using System;

namespace RabbitDemo.FailingConsumer
{
    public static class Program
    {
        public static void Main()
        {
            Console.Title = "FailingConsumer";
            using (var client = new RabbitClient(enableQos: true))
            {
                // Declare the exchange
                client.DeclareExchange(
                    "rd.durable", // exchange name
                    autoDelete: true,
                    durable: true);

                // Declare the queue
                client.DeclareQueue(
                    "rd_durable_queue",
                    durable: true,
                    autoDelete: false);

                // Bind queue to the exchange
                client.BindQueue("rd_durable_queue", "rd.durable", "");

                Console.WriteLine("Press ESC to exit...");

                // Subscribe to messages available from the queue
                using (client.Subscribe(OnMessage, queue: "rd_durable_queue", autoAck: false))
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
            Console.Write("Emit rejection? [y/n]: ");
            var yn = Console.ReadLine()?.Trim()?.ToLowerInvariant();
            if (yn == "y")
            {
                msg.Reject();
            }
            else
            {
                msg.Ack();
            }
        }
    }
}
