using Ether.Network.Core;
using Ether.Network.Packets;

namespace Ether.Network.Tests.Context
{
    internal class TestClient : NetConnection
    {
        public void SendHello()
        {
            using (var packet = new NetPacket())
            {
                packet.Write(0); // Hello header
                packet.Write(NetServerContext.WelcomeText);

                this.Send(packet);
            }
        }

        public override void HandleMessage(INetPacketStream packet)
        {
        }
    }
}
