namespace Message.MQ.Model
{
    using System.Collections.Generic;

    using Megadotnet.Message.MQ.IAdapter;

    using Moq;

    using Xunit;
    using BusniessEntities.Models;


    /// <summary>
    /// The imq adapter tests.
    /// </summary>
    public class MQAdapterTests
    {
        /// <summary>
        /// myMQAdapeter 
        /// </summary>
        private IMQAdapter<PushMessageModel> myMQAdapeter;
        private bool callbackflag = false;

        /// <summary>
        /// MQAdapterTests ctor
        /// </summary>
        public MQAdapterTests()
        {
            // assume
            var adapter = new Mock<IMQAdapter<PushMessageModel>>();
            var messages = new List<PushMessageModel>(){
               new PushMessageModel(){Id=1,  MSG_TITLE="title", MSG_CONTENT="content"}
               ,new PushMessageModel(){Id=2, MSG_TITLE="title2", MSG_CONTENT="content2"}
            };
            adapter.Setup(s => s.SendMessage<PushMessageModel>(It.IsAny<PushMessageModel>())).Returns(1);
            adapter.Setup(s => s.ReceviceMessage<PushMessageModel>())
                .Returns(messages.ToArray());

            adapter.Setup(s=>s.ReceviceListener<PushMessageModel>())
                .Callback(() => this.callbackflag=true);

            this.myMQAdapeter = adapter.Object;
        }

        /// <summary>
        /// The send message tests.
        /// </summary>
        [Fact]
        public void SendMessageTests()
        {
            // act
            int resultint = this.myMQAdapeter.SendMessage<string>("test message");
   
            // assert
            Assert.Equal(1, resultint);
        }

        /// <summary>
        /// Recevices the message should not be null.
        /// </summary>
        [Fact]
        public void ReceviceMessageShouldNotBeNull()
        {
            // act
            var results= this.myMQAdapeter.ReceviceMessage<PushMessageModel>();

            // assert
            Assert.NotNull(results);
            Assert.Equal(2, results.Length);

        }

        /// <summary>
        /// Recevices the listener should has call.
        /// </summary>
        [Fact]
        public void ReceviceListenerShouldHasCall()
        {
            // act
            this.myMQAdapeter.ReceviceListener < PushMessageModel>();

            // assert
            Assert.Equal(true, this.callbackflag);

        }


    }
}