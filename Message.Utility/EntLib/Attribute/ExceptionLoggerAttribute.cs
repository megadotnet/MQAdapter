using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messag.Utility.EntLib.Attribute
{
    internal class ExceptionLoggerAttribute : HandlerAttribute
    {
        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            return new ExceptionLoggerCallHandler();
        }
    }

    internal class ExceptionLoggerCallHandler : ICallHandler
    {
        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            IMethodReturn result = getNext()(input, getNext);
            if (result.Exception != null)
            {
                Console.WriteLine("Exception occured: " + result.Exception.Message);

                Console.WriteLine("Parameters:");
                foreach (var parameter in input.Arguments)
                {
                    Console.WriteLine(parameter.ToString());
                }

                Console.WriteLine("StackTrace:");
                Console.WriteLine(Environment.StackTrace);
            }

            return result;
        }

        public int Order { get; set; }
    }
}
