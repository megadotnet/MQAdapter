
using Messag.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace Messag.Utility.Helper
{
    public class MailHelper
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static ILogger logger;

        /// <summary>
        /// The rege x_ mail
        /// </summary>
        private readonly static string REGEX_MAIL = @"^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$";

        public MailHelper()
        {
            logger = new Logger.Logger(GetType());
           
        }

        /// <summary>
        /// Sends the mail.
        /// </summary>
        /// <param name="fromAddressStr">From address string.</param>
        /// <param name="toAddressStr">To address string.</param>
        /// <param name="fromPassword">From password.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        public static bool SendMail(string fromAddressStr, string toAddressStr, string fromPassword, string subject, string body)
        {
            var fromAddress = new MailAddress(fromAddressStr);

            var smtp = new SmtpClient
            {
                Host = "smtp.163.com",
                Port = 25,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage()
            {
                From = fromAddress,
                Subject = subject,
                Body = body
            })
            {
                // 收件人地址,可以设置多个
                ProcessMultiAddress(toAddressStr, message);

                //没有收件人直接返回
                if (message.To.Count <= 0)
                {
                    logger.DebugFormat("无效邮件收件人{0}", toAddressStr);
                    return false;
                }
                try
                {
                    smtp.Send(message);
                    return true;
                }
                catch (System.Exception ex)
                {
                    logger.Error(ex);

                }

                return true; ;

            }
        }

        /// <summary>
        /// Processes the multi address.
        /// </summary>
        /// <param name="toAddressStr">To address string.</param>
        /// <param name="message">The message.</param>
        private static void ProcessMultiAddress(string toAddressStr, MailMessage message)
        {
            if (toAddressStr.Contains(","))
            {
                foreach (var rec in toAddressStr.Split(','))
                {
                    if (Regex.IsMatch(rec, REGEX_MAIL))
                        message.To.Add(new MailAddress(rec));
                    else
                        logger.DebugFormat("无效邮件收件人{0}", rec);
                }
            }
            else
            {
                if (Regex.IsMatch(toAddressStr, REGEX_MAIL))
                    message.To.Add(new MailAddress(toAddressStr));
            }
        }
    }
}
