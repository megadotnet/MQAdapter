using BusniessEntities.Models;
using Messag.Utility.Config;
using Megadotnet.MessageMQ.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xunit;

namespace Message.MQ.Model
{
    /// <summary>
    /// ActiveMQ Instance Tests, it need to deploy ActiveMQInstance
    /// </summary>
    public class ActiveMQTests
    {

        /// <summary>
        /// The MQ_ ip_address
        /// </summary>
        private static string mq_Ip_address = MQConfig.MQIpAddress;
        /// <summary>
        /// The queu e_ destination
        /// </summary>
        private static string QUEUE_DESTINATION = MQConfig.QueueDestination;

        #region 发送消息到MQ
        /// <summary>
        /// Shoulds the send message for  model to MQ
        /// </summary>
        //[Fact]
        public void ShouldSendMessageForModelToMQ()
        {
            //assume
            var activemq = new ActiveMQAdapter<PushMessageModel>(mq_Ip_address, QUEUE_DESTINATION);
            var msg = CreateNewTestMessage();
             
            int flag = activemq.SendMessage<PushMessageModel>(msg);

            //assert
            Assert.Equal(1, flag);
        }

        /// <summary>
        /// Should  send ten message for model to mq.十条
        /// </summary>
        //[Fact]
        public void Should10SendMessageForUnionModelToMQ()
        {
            //assume
            var activemq = new ActiveMQAdapter<PushMessageModel>(mq_Ip_address, QUEUE_DESTINATION);

            for (int time = 0; time < 10; time++)
            {
                var msg = CreateNewTestMessage();

                int flag = activemq.SendMessage<PushMessageModel>(msg);
                //assert
                Assert.Equal(1, flag);

            }

        }


        //[Fact]
        public void ShouldSendMessagesToMQ()
        {
            //assume
            var activemq = new ActiveMQAdapter<PushMessageModel>(mq_Ip_address, QUEUE_DESTINATION);
            var msg = CreateNewTestMessage();
            var modellist = new PushMessageModel[] { msg, msg };
            int flag = activemq.SendMessages<PushMessageModel>(modellist);

            //assert
            Assert.Equal(1, flag);
        }


        /// <summary>
        /// ShouldSendMessagesStringToMQ
        /// </summary>
        //[Fact]
        public void ShouldSendMessagesStringToMQ()
        {
            //assume
            var activemq = new ActiveMQAdapter<string>(mq_Ip_address, QUEUE_DESTINATION);

            var modellist = new string[] { "msg", "22" };
            int flag = activemq.SendMessages<string>(modellist);

            //assert
            Assert.Equal(1, flag);
        }
     
        #endregion

        #region 收取MESSAGE从MQ
        /// <summary>
        /// Shoulds the send message model to mq and get them.
        /// </summary>
        //[Fact]
        public void ShouldSendMessageModelToMQAndGetThem()
        {
            //assume
            var activemq = new ActiveMQAdapter<PushMessageModel>(mq_Ip_address, QUEUE_DESTINATION);
            var msg = CreateNewTestMessage();


            //act
            activemq.SendMessage<PushMessageModel>(msg);

            Thread.Sleep(1000);
            var msglist = activemq.ReceviceMessage<PushMessageModel>();

            //assert
            Assert.NotNull(msglist);
            Assert.Equal(1, msglist.Length);

        }

        /// <summary>
        /// Shoulds the get message model from mq.
        /// </summary>
       // [Fact]
        public void ShouldGetMessageModelFromMQ()
        {
            //assume
            var activemq = new ActiveMQAdapter<PushMessageModel>(mq_Ip_address, QUEUE_DESTINATION);
      

            //act
            var msglist = activemq.ReceviceMessage<PushMessageModel>();

            //assert
            Assert.NotNull(msglist);
            Assert.True(msglist.Count()>0);
        }


        /// <summary>
        /// Shoulds the get message model from mq by listener.
        /// </summary>
        //[Fact]
        public void ShouldGetMessageModelFromMQByListener()
        {
            //assume
            var activemq = new ActiveMQAdapter<PushMessageModel>(mq_Ip_address, QUEUE_DESTINATION);

            //act & assert
            activemq.MQListener += m => {
                Assert.NotNull(m);
                Assert.Equal(1, m.Id);
            };
            activemq.ReceviceListener <PushMessageModel>();
        }

        #endregion


        /// <summary>
        /// Creates the new test message.
        /// </summary>
        /// <returns></returns>
        private PushMessageModel CreateNewTestMessage()
        {
            return new PushMessageModel()
            {
                Id = 1,
                MSGCONTENT = "Test Message" + DateTime.Now,
                MSGTITLE="Test Tile"
            };
        }
      
    }
}
