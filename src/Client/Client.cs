using Ether.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ether.Network.Packets;
using Ether.Network.Helpers;

namespace Client
{
    public class Client : NetClient
    {
        public Client()
            : base()
        {
        }

        public override void HandleMessage(NetPacketBase packet)
        {
            Console.WriteLine("Incoming message");

            //// RECIEVE PACKET ////

            //int packetSize = packet.Read<int>();
            //Console.WriteLine("==> Packet size: {0}", packetSize);

            var packetHeader = packet.Read<string>();
            Console.WriteLine("==> packet header: {0}", packetHeader);

            var packetContent = packet.Read<string>();

            Console.WriteLine("==> Packet content: {0}", packetContent);

            //// SEND PACKET ////

            var randomString = Helper.GenerateRandomString();

            var newPacket = new NetPacket();

            newPacket.Write("hello world!");
            newPacket.Write(randomString);

            this.Send(newPacket);

            base.HandleMessage(packet);
        }
    }
}
