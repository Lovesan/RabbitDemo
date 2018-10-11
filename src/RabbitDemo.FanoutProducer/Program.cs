using System;
using RabbitDemo.Common;

namespace RabbitDemo.FanoutProducer
{
    public static class Program
    {
        public static void Main()
        {
            Console.Title = "FanoutProducer";
            using (var client = new RabbitClient())
            {
                // Declare the exchange
                client.DeclareExchange(
                    "rd.fanout", // exchange name
                    type: "fanout",
                    autoDelete: true);

                while (true)
                {
                    Console.Write("Enter the message('exit' to quit): ");

                    var str = Console.ReadLine();
                    if (string.Equals(str, "exit", StringComparison.OrdinalIgnoreCase))
                        break;

                    // Publish the message
                    client.Publish(str, routingKey: "", exchange: "rd.fanout");
                }
            }
        }
    }
}
