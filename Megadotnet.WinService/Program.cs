using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Megadotnet.WinService
{
    /// <summary>
    /// Program
    /// </summary>
    class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
            var serviceToRun = new MQConsumeService();
            if (Environment.UserInteractive)
            {
                //监听MQ
                StartListenMQ();

                // serviceToRun.StartHangfireServer();

                Console.WriteLine("Press Enter to terminate Process ...");
                Console.ReadLine();

                // serviceToRun.StopHangfireServer();
            }
            else
            {
                //监听MQ
                StartListenMQ();
                ServiceBase.Run(serviceToRun);
            }
        }

        /// <summary>
        /// Starts the listen mq.
        /// </summary>
        private static void StartListenMQ()
        {
            new Core.MQHubsConfig().RegisterMQListenAndHubs();
        }
    }
}
