using System;

namespace DotnetCoreClientDemo
{
    /// <summary>
    /// Program
    /// </summary>
    public class Program
    {

        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("Prepare Connect MQ");

            var messageManager = new MessageManager();

            Console.WriteLine("Prepare Send message to MQ");
            if (messageManager.SendOneMessage())
            {
                Console.WriteLine("Done, press Any Key will be close");
            }
            else
            {
                Console.WriteLine("Fail");
            }

            Console.ReadKey();

        }

    }
}
