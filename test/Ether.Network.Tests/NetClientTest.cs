using System.Linq;
using System.Threading.Tasks;
using Ether.Network.Tests.Contexts.Echo;
using Xunit;

namespace Ether.Network.Tests
{
    public class NetClientTest
    {
        [Fact]
        public void ConnectClientToServer()
        {
            using (var server = new MyEchoServer())
            {
                server.Start();

                var client = new MyEchoClient("127.0.0.1", 4444, 512);

                client.Connect();
                Assert.True(client.ConnectedToServer);

                client.Disconnect();
                Assert.False(client.ConnectedToServer);

                client.Dispose();
            }
        }

        [Fact]
        public async Task ClientSendDataToServer()
        {
            await SendDataToServer(1);
        }

        [Fact]
        public async Task ClientSendDataToServer50()
        {
            await SendDataToServer(50);
        }

        [Fact]
        public async Task ClientSendDataToServer100()
        {
            await SendDataToServer(100);
        }

        [Fact]
        public async Task ClientSendDataToServer200()
        {
            await SendDataToServer(200);
        }

        [Fact]
        public async Task ClientSendDataToServer500()
        {
            await SendDataToServer(500);
        }

        [Fact]
        public async Task ClientSendDataToServer1000()
        {
            await SendDataToServer(1000);
        }

        private static async Task SendDataToServer(int messagesToSend)
        {
            using (var server = new MyEchoServer())
            {
                server.Start();

                var client = new MyEchoClient("127.0.0.1", 4444, 512);

                client.Connect();
                Assert.True(client.ConnectedToServer);

                for (var i = 0; i < messagesToSend; i++)
                {
                    client.SendRandomMessage();
                    await Task.Delay(10);
                }

                EchoClient clientFromServer = server.Clients.FirstOrDefault();
                Assert.NotNull(clientFromServer);

                // Wait for message
                while (clientFromServer.ReceivedData.Count < messagesToSend)
                    await Task.Delay(10);

                Assert.Equal(client.SendedData.Count, clientFromServer.ReceivedData.Count);

                for (var i = 0; i < messagesToSend; i++)
                {
                    string clientMessage = client.SendedData.ElementAt(i);
                    string serverMesssage = clientFromServer.ReceivedData.ElementAt(i);

                    Assert.NotNull(clientMessage);
                    Assert.NotNull(serverMesssage);
                    Assert.Equal(clientMessage, serverMesssage);
                }

                client.Disconnect();
                Assert.False(client.ConnectedToServer);
            }
        }
    }
}
