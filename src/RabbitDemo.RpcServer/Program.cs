using RabbitDemo.Common;
using System;

namespace RabbitDemo.RpcServer
{
    public static class Program
    {
        public static void Main()
        {
            Console.Title = "RpcServer";
            using (var client = new RabbitClient())
            {
                // Declare queue for receiving requests
                client.DeclareQueue("rd_rpc_queue");

                Console.WriteLine("Press ESC to exit...");

                // Subscribe to messages available from the queue
                using (client.Subscribe(OnMessage, queue: "rd_rpc_queue"))
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
            Console.WriteLine("Request received: {0}", str);

            // Make the result string
            var chars = str.ToCharArray();
            Array.Reverse(chars);
            var result = new string(chars);

            // Make a response
            msg.Client.Publish(
                result,
                routingKey: msg.ReplyTo,
                exchange: "",
                correlationId: msg.CorrelationId);
        }
    }
}
