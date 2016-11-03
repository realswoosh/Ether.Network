using Ether.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ether.Network.Packets;

namespace Client
{
    public class Client : NetClient
    {
        public Client()
            : base()
        {
        }

        public override void HandleMessage(NetPacket packet)
        {
            Console.WriteLine("Client has a packet to analyse");
            base.HandleMessage(packet);
        }
    }
}
