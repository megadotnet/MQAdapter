

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
    using System.Threading;
    using Messag.Utility.Config;


    /// <summary>
    /// ActiveMQListenAdapter. It is used for consumer roles.
    /// </summary>
    public class ActiveMQListenAdapter<T> : MQAdapterBase<T> where T:class
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILogger log = new Logger("ActiveMQListenAdapter");

        /// <summary>
        /// The factory
        /// </summary>
        private IConnectionFactory factory;
        /// <summary>
        /// The connection
        /// </summary>
        private IConnection connection;
        /// <summary>
        /// The session
        /// </summary>
        private ISession session;
        /// <summary>
        /// The dest
        /// </summary>
        private IDestination dest;
        /// <summary>
        /// The consumer
        /// </summary>
        private IMessageConsumer consumer;
        /// <summary>
        /// The client identifier
        /// </summary>
        private string clientId = "listener" + DateTime.Now.ToString("yyyymmddhhmmss");

        /// <summary>
        /// The address.
        /// </summary>
        private string IpAddress;

        /// <summary>
        /// The queu e_ destination.
        /// </summary>
        private string QUEUE_DESTINATION;

        /// <summary>
        /// The retry count
        /// </summary>
        private static UInt32 retryCount = 0;

        private bool fromRetryflag = false;

        #region .ctor

        public string IPAddress
        {
            set { IpAddress = value; }
        }

        public string QueueName
        {
            set { QUEUE_DESTINATION = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveMQListenAdapter"/> class.
        /// </summary>
        protected ActiveMQListenAdapter()
        {
            this.IpAddress = "tcp://localhost:61616/";
            QUEUE_DESTINATION = "PushMessageQueue";
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
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
        protected ActiveMQListenAdapter(string _address, string queueDest)
        {
            this.IpAddress = _address;
            QUEUE_DESTINATION = queueDest;
            factory = new ConnectionFactory(this.IpAddress);
            log.DebugFormat("准备连接ActiveMq IP:{0},Queue:{1}", this.IpAddress, QUEUE_DESTINATION);
        }

        /// <summary>
        /// The _instance
        /// </summary>
        private static ActiveMQListenAdapter<T> _instance;
        /// <summary>
        /// The _sync root
        /// </summary>
        private static readonly object _syncRoot = new object();

        /// <summary>
        /// Instances this instance.
        /// </summary>
        /// <returns></returns>
        public static ActiveMQListenAdapter<T> Instance(string _address, string queueDest)
        {
            // double-check locking
            if (_instance == null)
            {
                lock (_syncRoot)
                {
                    if (_instance == null)
                    {
                        // use lazy initialization
                        _instance = new ActiveMQListenAdapter<T>(_address, queueDest);
                    }
                }
            }

            return _instance;
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
                if (!IsConnected)
                    IsConnected = ReConnect();

                ProductMessageProcess<T>(session, t, queneName);
                flag = 1;

                connection.Stop();
                connection.Close();

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

            if (!IsConnected)
                IsConnected = ReConnect();
            ConsumeMessage<T>(session);

            return results;
        }



        /// <summary>
        /// The recevice listener.
        /// </summary>
        public override void ReceviceListener<T>() 
        {
            if (!IsConnected)
                IsConnected=ReConnect();

            ConsumeMessageByListener(session, OnMessageListener, needReleaseResource: false);

        }

        /// <summary>
        /// Called when [message listener].
        /// </summary>
        /// <param name="m">The m.</param>
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
        /// Creates the connection for listener.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="factory">The factory.</param>
        private IConnection CreateConnectionForListener(string clientId, IConnectionFactory factory)
        {
            try
            {
                connection = factory.CreateConnection();
                //if above method pass, clear count
                retryCount = 0;
                //set flag
                IsConnected=connection.IsStarted;
            }
            catch (NMSConnectionException ex)
            {
                log.Error(ex);
                //retry logic
                while (true)
                {
                    if (!IsConnected)
                    {
                        log.Debug("进入重试方法块");
                        log.DebugFormat("Current MQ connection status: {0}", IsConnected);
                        log.DebugFormat("准备第{0}次重新CreateConnection()", retryCount);
                        retryCount++;
                        Thread.Sleep(MQConfig.Interval);
                        fromRetryflag = true;
                        //递归
                        connection= CreateConnectionForListener(clientId, factory);
                    }
                    else
                    {
                        break;
                    }

                }
            }
            connection.ConnectionResumedListener += connection_ConnectionResumedListener;
            connection.ConnectionInterruptedListener += connection_ConnectionInterruptedListener;
            connection.ExceptionListener += connection_ExceptionListener;

            log.DebugFormat("ActiveMQ创建: {0}", connection.ToString());
            log.DebugFormat("ActiveMQ ClientID: {0}", clientId);

            connection.ClientId = clientId;
            connection.Start();
            IsConnected = connection.IsStarted;

            log.DebugFormat("ActiveMQ 已{0} IsStart:{1}", "connection.Start()", connection.IsStarted);

            //TODO:double check logic

            session = CreateSession(connection);
            connection.Start();
            
            if (fromRetryflag)
                ReceviceListener<T>();

            return connection;

        }

        /// <summary>
        /// Creates the session.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns></returns>
        private ISession CreateSession(IConnection connection)
        {
            log.DebugFormat("ActiveMQ 已{0}", "connection.CreateSession()");
            //connection.IsStarted
            return session = connection.CreateSession();
            
        }

        /// <summary>
        /// Connection_s the exception listener.
        /// </summary>
        /// <param name="exception">The exception.</param>
        void connection_ExceptionListener(Exception exception)
        {
            IsConnected = false;
            log.Debug("connection_ExceptionListener");
            log.Error(exception);

            IsConnected = ReConnect();
        }

        /// <summary>
        /// Res the connect.
        /// </summary>
        /// <returns></returns>
        private bool ReConnect()
        {
            connection = CreateConnectionForListener(clientId, factory);

            return connection.IsStarted;
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
        private static void ProductMessageProcess<T>(ISession session, T t,string queueName)
        {
            if (t is string)
            {
                SendText(session, t as string);
            }
            else
            {
                SendObject(session, t,queueName);
            }
        }

        /// <summary>
        /// The send object.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="t">
        /// The t.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        private static void SendObject<T>(ISession session, T t,string queueName)
        {
            //4.消息的目的地：destination
            IDestination dest = session.GetQueue(queueName);
            log.DebugFormat("ActiveMQ创建: {0}", dest.ToString());

            //5.创建用于发送消息的对象(设置其持久模式)
            using (IMessageProducer producer = session.CreateProducer(dest))
            {
                log.DebugFormat("ActiveMQ创建: {0}", producer.ToString());
                var objectMessage = producer.CreateObjectMessage(t);

                producer.Send(objectMessage);
                log.DebugFormat("ActiveMQ已发送: {0}",objectMessage.ToString());
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
        private static void SendText(ISession session, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("message text should be null");
            }

            // Create the Producer for the topic/queue   
            IMessageProducer prod = session.CreateProducer(new ActiveMQTopic("testing"));

            ITextMessage msg = prod.CreateTextMessage();
            msg.Text = text;
            Console.WriteLine("Sending: " + text);
            prod.Send(msg, MsgDeliveryMode.NonPersistent, MsgPriority.Normal, TimeSpan.MinValue);
        }

        /// <summary>
        /// The consume message by listener.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        private void ConsumeMessageByListener(ISession session, MessageListener mylistener)
        {
            ConsumeMessageByListener(session, mylistener,true);
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
             dest = session.GetQueue(QUEUE_DESTINATION);

            if (needReleaseResource)
            {
                ////5.创建用于接收消息的对象
                //using (IMessageConsumer consumer = session.CreateConsumer(dest))
                //{
                //    consumer.Listener += mylistener;
                //    //It is so important to keep listener's connection
                //    Console.ReadLine();
                //    log.Debug("开始 MQ Listener监听ing");
                //}
            }
            else
            {
                 consumer = session.CreateConsumer(dest);
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
        private T[] ConsumeMessage<T>(ISession session)   where T : class
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
                throw new Exception("Activemq connect fail",nmsConnectionExcepiton);
  
            }
            catch (Exception ex)
            {
                log.Error(ex);
                IsConnected = false;
                throw new Exception("Activemq connect fail", ex);
            }
        } 
        #endregion


        /// <summary>
        /// Sends the messages.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override int SendMessages<T>(T[] t)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sends the messages.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t">The t.</param>
        /// <param name="queneName"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override int SendMessages<T>(T[] t, string queneName)
        {
            throw new NotImplementedException();
        }
    }
}