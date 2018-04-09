// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MQMessageManagerBase.cs" company="Megadotnet">
// Copyright (c) 2010-2018 Petter Liu.  All rights reserved. 
// </copyright>
// <summary>
//  MQMessageManagerBase
// </summary>
// --------------------------------------------------------------------------------------------------------------------	

namespace Megadotnet.MessageMQ.Adapter.Managner
{
    using IronFramework.Common.Logging.Logger;
    using Megadotnet.MessageMQ.Adapter.NetCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// MQMessageManagerBase
    /// </summary>
    public abstract class MQMessageManagerBase
    {
        /// <summary>
        ///     The address
        /// </summary>
        protected static string mqAddress = MyMQConfig.MQIpAddress;

        /// <summary>
        ///     The queu e_ destination
        /// </summary>
        protected static string QUEUE_DESTINATION = MyMQConfig.QueueDestination;

        /// <summary>
        /// The log
        /// </summary>
        protected readonly ILogger log = new Logger("MQMessageManagerBase");
    }
}
