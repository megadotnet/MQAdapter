// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MQMessageManager.cs" company="Megadotnet">
// Copyright (c) 2010-2018 Petter Liu.  All rights reserved. 
// </copyright>
// <summary>
//  MQMessageManager
// </summary>
// --------------------------------------------------------------------------------------------------------------------	

namespace Megadotnet.MessageMQ.Adapter.Managner
{
    using Megadotnet.Message.MQ.IAdapter;
    using Megadotnet.MessageMQ.Adapter.NetCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// MQMessageManager
    /// </summary>
    /// <typeparam name="MessageModel">The type of the essage model.</typeparam>
    /// <seealso cref="Megadotnet.MessageMQ.Adapter.Managner.MQMessageManagerBase" />
    public class MQMessageManager<MessageModel> : MQMessageManagerBase where MessageModel : class
    {

        /// <summary>
        /// The retry count
        /// </summary>
        private static UInt32 retryCount = 0;

        /// <summary>
        /// The mqadapter
        /// </summary>
        private IMQAdapter<MessageModel> mqadapter = ActiveMQListenAdapter<MessageModel>
            .Instance(mqAddress, QUEUE_DESTINATION);

        /// <summary>
        /// Gets the active mq connect stauts.
        /// </summary>
        /// <returns></returns>
        public bool GetActiveMqConnectStauts()
        {
            return mqadapter.IsConnected;
        }

        /// <summary>
        /// Reads the message from mq.
        /// </summary>
        /// <returns>
        /// The <see cref="MessageModel[]"/>.
        /// </returns>
        public MessageModel[] ReadMessageFromMQ()
        {
            MessageModel[] messages = null;
            try
            {
                messages = mqadapter.ReceviceMessage<MessageModel>();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                if (!mqadapter.IsConnected)
                {
                    //pay attention
                    ReadMessageFromMQ();
                }
            }
            return messages;
        }

        /// <summary>
        /// Listens this instance.
        /// </summary>
        public void Listen(MQMessageListener<MessageModel> mqMessageListener)
        {

            mqadapter.MQListener += mqMessageListener;
            try
            {
                mqadapter.ReceviceListener<MessageModel>();
                //if above method pass, clear count
                retryCount = 0;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                //retry
                while (true)
                {
                    if (!mqadapter.IsConnected)
                    {
                        log.DebugFormat("准备第{0}次重新ReceviceListener()", retryCount);
                        retryCount++;
                        Thread.Sleep(MyMQConfig.Interval);
                        //递归
                        Listen(mqMessageListener);
                    }
                    else
                    {
                        break;
                    }
                }
            }

        }
    }
}
