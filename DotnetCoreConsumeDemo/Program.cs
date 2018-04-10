using System;

namespace DotnetCoreConsumeDemo
{
    /// <summary>
    /// Program
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("MQ Consumer App, Prepare listening for MQ");

            new MQHubsConfig().RegisterMQListenAndHubs();

            Console.WriteLine("Press Enter to terminate Process ...");
            Console.ReadLine();
        }
    }
}
