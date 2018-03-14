using System;

namespace DotnetCoreClientDemo
{
    class Program
    {


        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("Prepare Connect ");

            var messageManager = new MessageManager();

            if (messageManager.SendOneMessage())
            {
                Console.WriteLine("Done,press Any Key will be close");
            }
            else
            {
                Console.WriteLine("Fail");
            }

            Console.ReadKey();

        }

    }
}
