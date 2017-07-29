using Ether.Network;
using System;
using Ether.Network.Packets;

namespace SampleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new MyClient("127.0.0.1", 8888, 4096);

            client.Connect();

            Console.WriteLine("Enter a message and press enter...");
            while (true)
            {
                string input = Console.ReadLine();

                if (input == "quit")
                    break;

                Console.WriteLine("<- '{0}'", input);
                using (var packet = new NetPacket())
                {
                    packet.Write(input);
                    client.Send(packet);
                }
            }

            client.Disconnect();

            Console.WriteLine("Disconnected. Press any key to continue...");
            Console.ReadLine();
        }
    }

    class MyClient : NetClient
    {
        public MyClient(string host, int port, int bufferSize) 
            : base(host, port, bufferSize)
        {
        }

        protected override void HandleMessage(NetPacketBase packet)
        {
            var response = packet.Read<string>();

            Console.WriteLine("-> Server response: {0}", response);
        }

        protected override void OnConnected()
        {
            Console.WriteLine("Connected to {0}", this.Socket.RemoteEndPoint.ToString());
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine("Disconnected");
        }
    }
}