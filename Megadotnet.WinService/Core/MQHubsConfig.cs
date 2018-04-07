using Megadotnet.MessageMQ.Adapter.Managner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megadotnet.WinService.Core
{
    /// <summary>
    /// MQHubsConfig
    /// </summary>
    public class MQHubsConfig
    {
         private static readonly IronFramework.Common.Logging.Logger.ILogger logger 
            = new IronFramework.Common.Logging.Logger.Logger("MQHubsConfig");

        /// <summary>
        /// Registers the mq listen and hubs.
        /// </summary>
        public void RegisterMQListenAndHubs()
        {
      
            var mqMessageManager = new MQMessageManager<BusniessEntities.Models.PushMessageModel>();

            mqMessageManager.Listen(message =>
            {
                logger.DebugFormat("从MQ收到{0}", message.MSGCONTENT);
                //处理Logic

            });


        }
    }
}
