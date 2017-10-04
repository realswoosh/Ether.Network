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
                packet.Write(NetServerTest.WelcomeText);

                this.Send(packet);
            }
        }

        public override void HandleMessage(NetPacketBase packet)
        {
        }
    }
}
