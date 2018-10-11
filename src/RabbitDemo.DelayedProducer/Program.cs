using RabbitDemo.Common;
using System;

namespace RabbitDemo.DelayedProducer
{
    public static class Program
    {
        public static void Main()
        {
            Console.Title = "DelayedProducer";
            using (var client = new RabbitClient())
            {
                // Declare the exchange
                client.DeclareExchange(
                    "rd.delayed", // exchange name
                    autoDelete: true,
                    delayed: true);

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
                        exchange: "rd.delayed",
                        delayMs: 5000); // delay of 5 seconds
                }
            }
        }
    }
}
