using Ether.Network.Core;
using Ether.Network.Exceptions;
using Ether.Network.Tests.Context;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Ether.Network.Tests
{
    public class NetClientTest : NetServerContext
    {
        private INetClient ConnectClient()
        {
            var client = new MyClient("127.0.0.1", 4444, 512);

            client.Connect();
            //await this.DelayAction(5, 100, () => !client.IsConnected);

            return client;
        }

        [Fact]
        public void ConnectToServer()
        {
            using (INetClient client = this.ConnectClient())
            {
                Assert.Equal(true, client.IsConnected);
            }
        }

        public void SendPacketToServer()
        {
            using (INetClient client = this.ConnectClient())
            {
                if (!client.IsConnected)
                    throw new EtherDisconnectedException();

                // TODO Send packet and wait for answer
            }
        }
    }
}
