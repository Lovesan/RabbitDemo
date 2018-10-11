using System;
using System.Threading.Tasks;

namespace RabbitDemo.RpcClient
{
    public static class Program
    {
        public static async Task Main()
        {
            Console.Title = "RpcClient";
            using (var client = new RpcServerClient())
            {
                while (true)
                {
                    Console.Write("Enter the message('exit' to quit): ");

                    var str = Console.ReadLine();
                    if (string.Equals(str, "exit", StringComparison.OrdinalIgnoreCase))
                        break;

                    try
                    {
                        // Make a call
                        var result = await client.Call(str);
                        Console.WriteLine("Result: {0}\n", result);
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine("An error occured: {0}\n", e);
                    }
                }
            }
        }
    }
}
