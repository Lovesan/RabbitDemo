using RabbitDemo.Common;
using System;

namespace RabbitDemo.DurableProducer
{
    public static class Program
    {
        public static void Main()
        {
            Console.Title = "DurableProducer";
            using (var client = new RabbitClient())
            {
                // Declare the exchange
                client.DeclareExchange(
                    "rd.durable", // exchange name
                    autoDelete: true,
                    durable: true);

                while (true)
                {
                    Console.Write("Enter the message('exit' to quit): ");

                    var str = Console.ReadLine();
                    if (string.Equals(str, "exit", StringComparison.OrdinalIgnoreCase))
                        break;

                    // Publish the message
                    client.Publish(
                        str,
                        routingKey: "",
                        exchange: "rd.durable",
                        persistent: true);
                }
            }
        }
    }
}
