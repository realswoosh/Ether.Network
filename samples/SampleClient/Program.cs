using Ether.Network;
using System;
using Ether.Network.Packets;

namespace SampleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new MyClient("127.0.0.1", 8080, 4096);
            Console.WriteLine("Hello World!");
        }
    }

    class MyClient : NetClient
    {
        public MyClient(string host, int port, int bufferSize) 
            : base(host, port, bufferSize)
        {
        }

        public override void HandleMessage(NetPacketBase packet)
        {
            base.HandleMessage(packet);
        }
    }
}