using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BusniessEntities.Models
{
    [Serializable]
    [DataContract]
    public class PushMessageModel
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public int Id { get; set; }


        /// <summary>
        /// Gets or sets the content of the Message
        /// </summary>
        /// <value>
        /// The content of the Message
        /// </value>
        [DataMember]
        public string MSGCONTENT { get; set; }

        /// <summary>
        ///     消息标题
        /// </summary>
        [DataMember]
        public string MSGTITLE { get; set; }


        /// <summary>
        ///     类型 
        /// </summary>
        [DataMember]
        public string MSGTYPE { get; set; }


        /// <summary>
        /// Gets or sets the others.
        /// </summary>
        /// <value>
        /// The others.
        /// </value>
        [DataMember]
        public string[] Others { get; set; }
    }



}
