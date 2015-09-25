

namespace Megadotnet.MessageMQ.Adapter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Apache.NMS;
    using Apache.NMS.ActiveMQ;
    using Apache.NMS.ActiveMQ.Commands;
    using Megadotnet.Message.MQ.IAdapter;
    using System.Diagnostics;
    using Messag.Utility.Exception;
    using Messag.Logger;
    using System.Collections.Concurrent;

    /// <summary>
    /// The active mq adapter. It is mainly use send message method
    /// </summary>
    public class ActiveMQAdapter<T> : MQAdapterBase<T> where T : class
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILogger log = new Logger("ActiveMQAdapter");

        /// <summary>
        /// The address.
        /// </summary>
        private string IpAddress;


        /// <summary>
        /// The listener client identifier
        /// </summary>
        private string listenerClientID = "listener" + DateTime.Now.ToString("yyyymmddhhmmss");

        /// <summary>
        /// The sender identifier
        /// </summary>
        private string senderID = "sender" + DateTime.Now.ToString("yyyymmddhhmmss");


        #region .ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveMQAdapter"/> class.
        /// </summary>
        public ActiveMQAdapter()
        {
            this.IpAddress = "tcp://localhost:61616/";
            QUEUE_DESTINATION = "MessageCenterQueue";
        }

        public override bool IsConnected { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveMQAdapter"/> class.
        /// </summary>
        /// <param name="_address">
        /// The _address.
        /// </param>
        /// <param name="queueDest">
        /// The queue dest.
        /// </param>
        public ActiveMQAdapter(string _address, string queueDest)
        {
            this.IpAddress = _address;
            QUEUE_DESTINATION = queueDest;
        }

        #endregion

        #region Member method

        /// <summary>
        /// The send message.
        /// </summary>
        /// <param name="t">
        /// The t.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int SendMessage<T>(T t)
        {
            return SendMessage<T>(t, QUEUE_DESTINATION);
        }

        /// <summary>
        /// Sends the messages.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public override int SendMessages<T>(T[] t)
        {
            return SendMessages<T>(t, QUEUE_DESTINATION);
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t">The t.</param>
        /// <param name="queneName">Name of the quene.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override int SendMessage<T>(T t, string queneName)
        {
            int flag = 0;
            TryCatchGeneralExceptionWrapper(() =>
            {
                this.CreateConnection(session =>
                {
                    ProductMessageProcess<T>(session, new T[] { t }, queneName);
                    flag = 1;
                }, senderID);

            });
            return flag;
        }

        public override int SendMessages<T>(T[] t, string queneName)
        {
            int flag = 0;
            TryCatchGeneralExceptionWrapper(() =>
            {
                this.CreateConnection(session =>
                {
                    ProductMessageProcess<T>(session, t, queneName);
                    flag = 1;
                }, senderID);

            });
            return flag;
        }

        /// <summary>
        /// The recevice message.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T[]"/>.
        /// </returns>
        public override T[] ReceviceMessage<T>()
        {
            T[] results = new T[] { };
            this.CreateConnection(session => { results = this.ConsumeMessage<T>(session); }
                , listenerClientID);

            return results;
        }

        /// <summary>
        /// The recevice listener.
        /// </summary>
        public override void ReceviceListener<T>()
        {
            this.CreateConnection(
                session =>
                {
                    this.ConsumeMessageByListener(session, OnMessageListener, needReleaseResource: false);
                },
                "listener", needReleaseConnection: false);
        }

        private void OnMessageListener(IMessage m)
        {
            var objectMessage = m as IObjectMessage;
            if (objectMessage != null)
            {
                log.Debug("current objectMessage is not null");
                var msg = objectMessage.Body as T;

                this.MQListener(msg);
            }
        }

        /// <summary>
        /// Occurs when [mq listener].
        /// </summary>
        public override event MQMessageListener<T> MQListener;
        #endregion

        #region private helper method

        /// <summary>
        /// The create connection.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="clientId">
        /// The client id.
        /// </param>
        private void CreateConnection(Action<ISession> action, string clientId)
        {
            CreateConnection(action, clientId, true);
        }

        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="needReleaseConnection">if set to <c>true</c> [need release connection].</param>
        private void CreateConnection(Action<ISession> action, string clientId, bool needReleaseConnection)
        {
            TryCatchGeneralExceptionWrapper(() =>
            {
                //1.连接工厂：用于创建连接
                IConnectionFactory factory = new ConnectionFactory(this.IpAddress);
                log.DebugFormat("ActiveMQ创建: {0}, 地址 {1}", factory.ToString(), this.IpAddress);
                if (needReleaseConnection)
                {
                    //2.JMS客户端到JMS Provider的连接
                    using (IConnection connection = factory.CreateConnection())
                    {
                        log.DebugFormat("ActiveMQ创建: {0}", connection.ToString());

                        connection.ClientId = clientId;
                        //建立连接
                        connection.Start();

                        if (connection.IsStarted)
                            IsConnected = true;

                        // Create the Session   3.线程：同时设置是否支持事务和acknowledge标识
                        using (ISession session = connection.CreateSession())
                        {
                            action(session);
                        }

                        connection.Stop();

                        log.Debug("ActiveMQ connect连接stop停止");

                        connection.Close();

                        log.Debug("ActiveMQ connect连接Close关闭");
                    }
                }
                else
                {
                    CreateConnectionForListener(action, clientId, factory);
                }

            });
        }

        /// <summary>
        /// Creates the connection for listener.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="factory">The factory.</param>
        private void CreateConnectionForListener(Action<ISession> action, string clientId, IConnectionFactory factory)
        {
            IConnection connection = factory.CreateConnection();
            connection.ConnectionResumedListener += connection_ConnectionResumedListener;
            connection.ConnectionInterruptedListener += connection_ConnectionInterruptedListener;
            connection.ExceptionListener += connection_ExceptionListener;

            log.DebugFormat("ActiveMQ创建: {0}", connection.ToString());
            log.DebugFormat("ActiveMQ ClientID: {0}", clientId);

            connection.ClientId = clientId;
            connection.Start();

            log.DebugFormat("ActiveMQ 已{0} IsStart:{1}", "connection.Start()", connection.IsStarted);

            //connection.IsStarted
            ISession session = connection.CreateSession();


            log.DebugFormat("ActiveMQ 已{0}", "connection.CreateSession()");

            action(session);
        }

        /// <summary>
        /// Connection_s the exception listener.
        /// </summary>
        /// <param name="exception">The exception.</param>
        void connection_ExceptionListener(Exception exception)
        {
            log.Debug("connection_ExceptionListener");
            log.Error(exception);
            IsConnected = false;
            throw exception;
        }

        /// <summary>
        /// Connection_s the connection interrupted listener.
        /// </summary>
        void connection_ConnectionInterruptedListener()
        {
            log.Debug("ConnectionInterruptedListener");
            IsConnected = false;
        }

        /// <summary>
        /// Connection_s the connection resumed listener.
        /// </summary>
        void connection_ConnectionResumedListener()
        {
            log.Debug("connection_ConnectionResumedListener");
            IsConnected = true;
        }

        #endregion

        #region private logic method

        /// <summary>
        /// The product message process.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="t">
        /// The t.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        private void ProductMessageProcess<T>(ISession session, T[] t, string queueName)
        {
            if (t.GetType().ToString() == "System.String[]")
            {
                SendTexts(session: session, texts: t as string[], topicName: QUEUE_DESTINATION);
            }
            //send with objects
            else
            {
                SendObject(session, t, queueName);
            }
        }


        /// <summary>
        /// Sends the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session">The session.</param>
        /// <param name="t">The t.</param>
        /// <param name="queueName">Name of the queue.</param>
        private static void SendObject<T>(ISession session, T[] t, string queueName)
        {
            //4.消息的目的地：destination
            IDestination dest = session.GetQueue(queueName);
            log.DebugFormat("ActiveMQ创建: {0}", dest.ToString());

            //5.创建用于发送消息的对象(设置其持久模式)
            using (IMessageProducer producer = session.CreateProducer(dest))
            {
                foreach (var msg in t)
                {
                    log.DebugFormat("ActiveMQ创建: {0}", producer.ToString());
                    var objectMessage = producer.CreateObjectMessage(msg);

                    producer.Send(objectMessage);
                    log.DebugFormat("ActiveMQ已发送: {0}", msg);
                }


            }
        }

        /// <summary>
        /// The send text.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        private static void SendTexts(ISession session, string[] texts, string topicName)
        {
            if (texts == null && texts.Length <= 0)
            {
                throw new ArgumentNullException("message array text should be null");
            }

            //if (string.IsNullOrEmpty(text))
            //{
            //    throw new ArgumentNullException("message text should be null");
            //}

            // Create the Producer for the topic/queue   
            IMessageProducer prod = session.CreateProducer(new ActiveMQTopic(topicName));

            foreach (var text in texts)
            {
                ITextMessage msg = prod.CreateTextMessage();
                msg.Text = text;
                Debug.WriteLine("Sending: " + text);
                prod.Send(msg, MsgDeliveryMode.NonPersistent, MsgPriority.Normal, TimeSpan.MinValue);
            }

        }

        /// <summary>
        /// The consume message by listener.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        private void ConsumeMessageByListener(ISession session, MessageListener mylistener)
        {
            ConsumeMessageByListener(session, mylistener, true);
        }

        /// <summary>
        /// Consumes the message by listener.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="mylistener">The mylistener.</param>
        /// <param name="needReleaseResource">if set to <c>true</c> [need release resource].</param>
        private void ConsumeMessageByListener(ISession session, MessageListener mylistener, bool needReleaseResource)
        {
            // Create the Consumer  4.消息的目的地：destination
            IDestination dest = session.GetQueue(QUEUE_DESTINATION);

            if (needReleaseResource)
            {
                //5.创建用于接收消息的对象
                using (IMessageConsumer consumer = session.CreateConsumer(dest))
                {
                    consumer.Listener += mylistener;
                    //It is so important to keep listener's connection
                    Console.ReadLine();
                    log.Debug("开始 MQ Listener监听ing");
                }
            }
            else
            {
                IMessageConsumer consumer = session.CreateConsumer(dest);
                log.DebugFormat("ActiveMQ创建: {0}", consumer.ToString());
                consumer.Listener += mylistener;
                log.Debug("不释放资源模式 ：开始 MQ Listener监听ing");

            }
        }
        /// <summary>
        /// The consume message.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T[]"/>.
        /// </returns>
        private T[] ConsumeMessage<T>(ISession session) where T : class
        {
            T targetObject = default(T);
            IDestination dest = session.GetQueue(QUEUE_DESTINATION);
            BlockingCollection<T> list = new BlockingCollection<T>();
            using (IMessageConsumer consumer = session.CreateConsumer(dest))
            {
                IMessage message;
                //Pull Model
                while ((message = consumer.Receive(TimeSpan.FromMilliseconds(2000))) != null)
                {
                    var objectMessage = message as IObjectMessage;
                    if (objectMessage != null)
                    {
                        targetObject = objectMessage.Body as T;
                        list.Add(targetObject);
                    }
                    else
                    {
                        Console.WriteLine("Object Message is null");
                    }
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// lister
        /// </summary>
        /// <param name="message">
        /// </param>
        private static void consumer_Listener(IMessage message)
        {
            try
            {
                ITextMessage msg = (ITextMessage)message;
                Console.WriteLine("Receive: " + msg.Text);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        #endregion

        #region Private help method


        /// <summary>
        /// Tries the catch general exception wrapper.
        /// </summary>
        /// <param name="action">The action.</param>
        private void TryCatchGeneralExceptionWrapper(Action action)
        {
            try
            {
                action();
            }
            catch (Apache.NMS.NMSConnectionException nmsConnectionExcepiton)
            {
                log.Error(nmsConnectionExcepiton);
                IsConnected = false;

            }
            catch (Exception ex)
            {
                log.Error(ex);
                IsConnected = false;

            }
        }
        #endregion




    }
}