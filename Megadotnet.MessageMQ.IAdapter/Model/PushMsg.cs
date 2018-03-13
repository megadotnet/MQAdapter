using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BusniessEntities.Models
{
    /// <summary>
    /// PushMsg
    /// </summary>
    [Serializable]
    [DataContract]
    public class PushMsg : PushMessageModel
    {
        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        /// <value>
        /// The users.
        /// </value>
        [DataMember]
        public string[] Users { get; set; }

        /// <summary>
        /// Gets or sets the type of the MSG send  
        /// P推送，S短信， M邮件
        /// </summary>
        /// <value>
        /// The type of the MSG send.
        /// </value>
        [DataMember]
        public string MsgSendType { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether this instance is read.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is read; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool IsRead { get; set; }


        /// <summary>
        /// Gets or sets the expiration time.
        /// </summary>
        /// <value>
        /// The expiration time.
        /// </value>
        [DataMember]
        public DateTime ExpirationTime { get; set; }

    }


}
