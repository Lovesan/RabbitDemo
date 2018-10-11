using RabbitDemo.Common;
using System;

namespace RabbitDemo.SimpleProducer
{
    public static class Program
    {
        public static void Main()
        {
            Console.Title = "SimpleProducer";
            using (var client = new RabbitClient())
            {
                // Declare the queue
                client.DeclareQueue("rd_simple_queue");

                while (true)
                {
                    Console.Write("Enter the message('exit' to quit): ");

                    var str = Console.ReadLine();
                    if(string.Equals(str, "exit", StringComparison.OrdinalIgnoreCase)) 
                        break;

                    // Publish the message
                    client.Publish(str, routingKey: "rd_simple_queue", exchange: "");
                }
            }
        }
    }
}
