// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MQAdapterBase.cs" company="Megadotnet">
//   Copyright (c) 2010-2017 Petter Liu.  All rights reserved. 
// </copyright>
// <summary>
//   The repository test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Megadotnet.Message.MQ.IAdapter
{
    /// <summary>
    /// The mq adapter base.
    /// </summary>
    public abstract class MQAdapterBase<TEntiy> : IMQAdapter<TEntiy> where TEntiy:class
    {
        /// <summary>
        /// Gets or sets the queu e_ destination.
        /// </summary>
        /// <value>
        /// The queu e_ destination.
        /// </value>
        public string QUEUE_DESTINATION
        {
            get;
            set;
        }


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
        public abstract int SendMessage<T>(T t);


        /// <summary>
        /// Sends the messages.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public abstract int SendMessages<T>(T[] t);

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t">The t.</param>
        /// <param name="queneName">Name of the quene.</param>
        /// <returns></returns>
        public abstract int SendMessage<T>(T t, string queneName);


        /// <summary>
        /// Sends the messages.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t">The t.</param>
        /// <param name="queneName">Name of the quene.</param>
        /// <returns></returns>
        public abstract int SendMessages<T>(T[] t, string queneName);

        /// <summary>
        /// The recevice message.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TEntiy[]"/>.
        /// </returns>
        public abstract T[] ReceviceMessage<T>() where T : class;

        /// <summary>
        /// The recevice listener.
        /// </summary>
        public abstract void ReceviceListener<T>() where T : class;


        /// <summary>
        /// Occurs when [mq listener].
        /// </summary>
        public abstract event MQMessageListener<TEntiy> MQListener;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public abstract bool IsConnected { get; set; }
    }
}