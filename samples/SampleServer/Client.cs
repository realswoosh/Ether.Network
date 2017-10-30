using Ether.Network;
using System;
using Ether.Network.Packets;
using Ether.Network.Interfaces;

namespace SampleServer
{
    internal sealed class Client : NetUser
    {
        /// <summary>
        /// Send hello to the incoming clients.
        /// </summary>
        public void SendFirstPacket()
        {
            using (var packet = new NetPacket())
            {
                packet.Write("Welcome " + this.Id.ToString());

                //this.Send(packet);
            }
        }

        /// <summary>
        /// Receive messages from the client.
        /// </summary>
        /// <param name="packet"></param>
        public override void HandleMessage(INetPacketStream packet)
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