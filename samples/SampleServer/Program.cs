using Ether.Network;
using System;

namespace SampleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new SampleServer();
            server.Start();

            Console.ReadLine();
        }
    }

    class SampleServer : NetServer<Client>
    {
        public SampleServer()
        {
            this.Configuration.Backlog = 100;
            this.Configuration.Port = 4444;
            this.Configuration.MaximumNumberOfConnections = 100;
            this.Configuration.Host = "127.0.0.1";
        }

        protected override void Initialize()
        {
        }
    }

    class Client : NetConnection
    {
    }
}