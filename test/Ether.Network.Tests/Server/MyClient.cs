using System;
using Ether.Network.Packets;

namespace Ether.Network.Tests.Server
{
    internal class MyClient : NetClient
    {
        public MyClient(string host, int port, int bufferSize) 
            : base(host, port, bufferSize)
        {
        }

        protected override void HandleMessage(NetPacketBase packet)
        {
            var header = packet.Read<int>();

            switch (header)
            {
                case 0:
                    var message = packet.Read<string>();
                    Console.WriteLine("Received: {0}", message);
                    break;
            }
        }

        protected override void OnConnected()
        {
        }

        protected override void OnDisconnected()
        {
        }
    }
}
