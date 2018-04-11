
using IronFramework.Common.Logging.Logger;
using System;

namespace DotnetCoreConsumeDemo
{
    /// <summary>
    /// Program
    /// </summary>
    class Program
    {
        private static readonly ILogger logger = new Logger(typeof(Program));

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
            logger.Trace("MQ Consumer App, Prepare listening for MQ");
            Console.WriteLine("MQ Consumer App, Prepare listening for MQ");

            new MQHubsConfig().RegisterMQListenAndHubs();

            Console.WriteLine("Press Enter to terminate Process ...");
            Console.ReadLine();
        }
    }
}
