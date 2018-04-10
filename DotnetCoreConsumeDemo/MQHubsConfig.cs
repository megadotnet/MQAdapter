using IronFramework.Common.Logging.Logger;
using Megadotnet.MessageMQ.Adapter.Managner;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetCoreConsumeDemo
{
    /// <summary>
    /// MQHubsConfig
    /// </summary>
    public class MQHubsConfig
    {
        private static readonly ILogger logger = new Logger("MQHubsConfig");

        /// <summary>
        /// Registers the mq listen and hubs.
        /// </summary>
        public void RegisterMQListenAndHubs()
        {

            var mqMessageManager = new MQMessageManager<BusniessEntities.Models.PushMessageModel>();

            mqMessageManager.Listen(message =>
            {
                Console.WriteLine(string.Format("从MQ收到{0}", message.MSGCONTENT));
                logger.DebugFormat("从MQ收到{0}", message.MSGCONTENT);
                //处理Logic

            });


        }
    }
}
