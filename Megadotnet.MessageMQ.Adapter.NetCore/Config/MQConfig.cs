

namespace Megadotnet.MessageMQ.Adapter.NetCore
{
    using IronFramework.Common.Config;
    using System;

    /// <summary>
    /// MQConfig
    /// </summary>
    /// <seealso cref="IronFramework.Common.Config.BaseConfig" />
    public class MyMQConfig : BaseConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyMQConfig"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        public MyMQConfig(System.Xml.XmlNode node)
            : base(node)
        {

        }

        /// <summary>
        /// Initializes the <see cref="MyMQConfig"/> class.
        /// </summary>
        static MyMQConfig()
        {
            ConfigSectionName = typeof(MyMQConfig).Name;
        }

        /// <summary>
        /// Gets the mq ip address.
        /// </summary>
        /// <value>
        /// The mq ip address.
        /// </value>
        public static string MQIpAddress
        {
            get
            {
                return configSection["mqidaddress"];
            }
        }

        /// <summary>
        /// Gets the queue destination.
        /// </summary>
        /// <value>
        /// The queue destination.
        /// </value>
        public static string QueueDestination
        {
            get
            {
                return configSection["queuedestination"];
            }
        }

        /// <summary>
        /// Gets the interval.
        /// </summary>
        /// <value>
        /// The interval.
        /// </value>
        public static int Interval
        {
            get
            {
                var myvalue = configSection["interval"];
                return Convert.ToInt32(myvalue);
            }
        }

        /// <summary>
        /// Gets the name of the user for MQ connection
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public static string UserName
        {
            get
            {
                return configSection["username"];
            }
        }

        /// <summary>
        /// Gets the password  for MQ connection
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public static string Password
        {
            get
            {
                return configSection["password"];
            }
        }
    }
}
