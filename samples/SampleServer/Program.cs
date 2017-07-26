using Ether.Network;
using System;
using Ether.Network.Packets;

namespace SampleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new SampleServer();
            server.Start();
        }
    }

    class SampleServer : NetServer<Client>
    {
        public SampleServer()
        {
            this.Configuration.Backlog = 100;
            this.Configuration.Port = 8888;
            this.Configuration.MaximumNumberOfConnections = 100;
            this.Configuration.Host = "127.0.0.1";
        }

        protected override void Initialize()
        {
            Console.WriteLine("Initializing the server.");
        }

        protected override void OnClientConnected()
        {
            Console.WriteLine("New client connected!");
        }

        protected override void OnClientDisconnected()
        {
            Console.WriteLine("Client disconnected!");
        }
    }

    class Client : NetConnection
    {
        public override void Greetings()
        {
            using (var packet = new NetPacket())
            {
                packet.Write(1);

                this.Send(packet);
            }
        }

        public override void HandleMessage(NetPacketBase packet)
        {
        }
    }
}