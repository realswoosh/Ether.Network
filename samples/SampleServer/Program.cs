using Ether.Network;
using System;
using Ether.Network.Packets;

namespace SampleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Ether.Network Server";

            using (var server = new SampleServer())
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
            Console.WriteLine("Server is ready.");
        }

        protected override void OnClientConnected(Client connection)
        {
            Console.WriteLine("New client connected!");

            connection.SendFirstPacket();
        }

        protected override void OnClientDisconnected(Client connection)
        {
            Console.WriteLine("Client disconnected!");
        }
    }

    class Client : NetConnection
    {
        public void SendFirstPacket()
        {
            using (var packet = new NetPacket())
            {
                packet.Write("Hello world!");

                this.Send(packet);
            }
        }

        public override void HandleMessage(NetPacketBase packet)
        {
            string value = packet.Read<string>();

            Console.WriteLine("Received '{1}' from {0}", this.Id, value);

            using (var p = new NetPacket())
            {
                p.Write(string.Format("OK: '{0}'", value));
                this.Send(p);
            }
        }
    }
}