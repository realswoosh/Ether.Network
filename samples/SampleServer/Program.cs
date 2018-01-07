using System;

namespace SampleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Ether.Network Server";
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            using (var server = new SampleServer())
                server.Start();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
        }
    }
}