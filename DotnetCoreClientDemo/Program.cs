using BusniessEntities.Models;
using Megadotnet.MessageMQ.Adapter;
using Megadotnet.MessageMQ.Adapter.Config;
using System;

namespace DotnetCoreClientDemo
{
    class Program
    {
        /// <summary>
        /// The MQ_ ip_address
        /// </summary>
        private static string mq_Ip_address = MyMQConfig.MQIpAddress;
        /// <summary>
        /// The queu e_ destination
        /// </summary>
        private static string QUEUE_DESTINATION = MyMQConfig.QueueDestination;

        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("Begin Connect " + typeof(MyMQConfig).Name);
         

            var activemq = new ActiveMQAdapter<PushMessageModel>(mq_Ip_address, QUEUE_DESTINATION);
            var msg = CreateNewTestMessage();

            int flag = activemq.SendMessage<PushMessageModel>(msg);

            if (flag>0)
            {
                Console.WriteLine("Done,press Any Key will be close");
            }
            else
            {
                Console.WriteLine("Fail");
            }

            Console.ReadKey();

        }

        /// <summary>
        /// Creates the new test message.
        /// </summary>
        /// <returns></returns>
        private static PushMessageModel CreateNewTestMessage()
        {
            return new PushMessageModel()
            {
                Id = 1,
                MSGCONTENT = "Test Message" + DateTime.Now,
                MSGTITLE = "Test Tile"
            };
        }
    }
}
