using Ether.Network.Exceptions;
using Ether.Network.Tests.Context;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Ether.Network.Tests
{
    public class NetServerTest : NetServerContext
    {
        private async Task<INetClient> ConnectClient()
        {
            var client = new MyClient("127.0.0.1", 4444, 512);

            client.Connect();
            await this.DelayAction(5, 100, () => !client.IsConnected);

            return client;
        }

        [Fact]
        public async void ConnectToServer()
        {
            INetClient client = await this.ConnectClient();

            Assert.Equal(true, client.IsConnected);

            client.Disconnect();
        }

        public async void SendPacketToServer()
        {
            INetClient client = await this.ConnectClient();

            if (!client.IsConnected)
                throw new EtherDisconnectedException();

            // TODO Send packet and wait for answer

            client.Disconnect();
        }
    }
}
