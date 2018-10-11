using System;
using RabbitDemo.Common;

namespace RabbitDemo.SimpleConsumer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "SimpleConsumer";
            using (var client = new RabbitClient())
            {
                // Declare a queue
                client.DeclareQueue("rd_simple_queue");

                Console.WriteLine("Press ESC to exit...");

                // Subscribe to messages available from queue
                using (client.Subscribe(OnMessage, queue: "rd_simple_queue"))
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
