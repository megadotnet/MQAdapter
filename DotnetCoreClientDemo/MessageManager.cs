
namespace DotnetCoreClientDemo
{
    using BusniessEntities.Models;
    using Megadotnet.Message.MQ.IAdapter;
    using Megadotnet.MessageMQ.Adapter;
    using Megadotnet.MessageMQ.Adapter.NetCore;
    using System;

    public class MessageManager
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
        /// Sends the one message.
        /// </summary>
        /// <returns></returns>
        public bool SendOneMessage()
        {
            IMQAdapter<PushMessageModel> activemq = new ActiveMQAdapter<PushMessageModel>(mq_Ip_address, QUEUE_DESTINATION);
            var msg = CreateNewTestMessage();

            int flag = activemq.SendMessage<PushMessageModel>(msg);
            return flag > 0;
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
