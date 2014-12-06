
namespace Megadotnet.Message.MQ.IAdapter
{
    /// <summary>
    /// The mq adapter base.
    /// </summary>
    public abstract class MQAdapterBase<T> : IMQAdapter<T> where T:class
    {
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
        /// Sends the message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t">The t.</param>
        /// <param name="queneName">Name of the quene.</param>
        /// <returns></returns>
        public abstract int SendMessage<T>(T t,string queneName);

        /// <summary>
        /// The recevice message.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T[]"/>.
        /// </returns>
        public abstract T[] ReceviceMessage<T>()  where T : class;

        /// <summary>
        /// The recevice listener.
        /// </summary>
        public abstract void ReceviceListener<T>()  where T : class;


        /// <summary>
        /// Occurs when [mq listener].
        /// </summary>
        public abstract event MQMessageListener<T> MQListener;

/// <summary>
/// Gets or sets a value indicating whether this instance is connected.
/// </summary>
/// <value>
/// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
/// </value>
        public abstract bool IsConnected { get; set; }
    }
}