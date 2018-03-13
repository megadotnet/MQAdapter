using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusniessEntities.Models
{
    public enum MsgSendType : int
    {
        /// <summary>
        ///  短信
        /// </summary>
        S,
        /// <summary>
        /// 邮件
        /// </summary>
        M,
        /// <summary>
        /// 推送
        /// </summary>
        P
    }
}
