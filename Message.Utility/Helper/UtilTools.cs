using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Message.Utility.Helper
{
    public class UtilTools
    {
        /// <summary>
        /// Get guest IP address
        /// </summary>
        /// <returns>string</returns>
        public static string GetGuestIP()
        {
            string guestIP = string.Empty;
            var currentContext = HttpContext.Current;
            if (currentContext != null)
            {
                guestIP = currentContext.Request.UserHostAddress;
            }
            return guestIP;
        }


        /// <summary>
        /// MergeSetsAndDoAction
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uIPrivilegeIds">The u i privilege ids.</param>
        /// <param name="dbPrivilegeIDs">The database privilege i ds.</param>
        /// <param name="delAction">The delete action.</param>
        /// <param name="createAction">The create action.</param>
        public static bool MergeSetsAndDoAction<T>(T[] uIPrivilegeIds, T[] dbPrivilegeIDs, Action<HashSet<T>> delAction, Action<HashSet<T>> createAction)
        {
            bool flag = false;
            var uIPrivilegeSets = new HashSet<T>(uIPrivilegeIds);
            var dbPrivilegeSets = new HashSet<T>(dbPrivilegeIDs);

            uIPrivilegeSets.SymmetricExceptWith(dbPrivilegeSets);
            //uIPrivilegeIDs.ToList().ForEach(dd => Console.WriteLine(dd));

            dbPrivilegeSets.IntersectWith(uIPrivilegeSets);
            // dbPrivilegeIDs.ToList().ForEach(dd => Console.WriteLine("for delete {0}", dd));
            using (var tsCope = new System.Transactions.TransactionScope())
            {
                //delete action 
                if (dbPrivilegeSets.Count != 0)
                {
                    delAction(dbPrivilegeSets);

                    uIPrivilegeSets.ExceptWith(dbPrivilegeSets);

                }

                //add action
                if (uIPrivilegeSets.Count != 0)
                {
                    createAction(uIPrivilegeSets);
                }
                tsCope.Complete();
            }
            flag = true;
            return flag;

        }
    }
}