// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MQConfig.cs" company="Megadotnet">
// Copyright (c) 2010-2018 Petter Liu.  All rights reserved. 
// </copyright>
// <summary>
//  MQConfig
// </summary>
// --------------------------------------------------------------------------------------------------------------------	
namespace Messag.Utility.Config
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// MQ Configuraion setions class
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    ///  string address = Config.MQIpAddress;
    /// ]]>
    /// </code>
    /// </example>
    public class MQConfig
    {
        private System.Xml.XmlNode m_section;
        private static readonly string ConfigSectionName = "MyActiveMQ";

        /// <summary>
        /// Initializes a new instance of the <see cref="MQConfig"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        public MQConfig(System.Xml.XmlNode node)
        {
            m_section = node;
        }

        /// <summary>
        /// Gets the configuration section.
        /// </summary>
        /// <value>
        /// The configuration section.
        /// </value>
        /// <exception cref="System.ApplicationException">Failed to get configuration from App.config.</exception>
        private static MQConfig configSection
        {
            get
            {
                MQConfig config = (MQConfig)System.Configuration.ConfigurationManager.GetSection(ConfigSectionName);
                if (config == null)
                {
                    throw new ApplicationException("Failed to get configuration from App.config.");
                }
                return config;
            }
        }

        /// <summary>
        /// Gets the <see cref="System.String"/> with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="System.String"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public string this[string key]
        {
            get
            {
                System.Xml.XmlNode node = m_section.SelectSingleNode(key);
                if (node != null)
                    return node.InnerText;
                else
                    return null;
            }
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
