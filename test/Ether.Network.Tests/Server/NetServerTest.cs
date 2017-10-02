using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Threading.Tasks;
using System.Threading;

namespace Ether.Network.Tests.Server
{
    public class NetServerTest : IAsyncLifetime
    {
        public static readonly string WelcomeText = "Welcome to the Test!";
        private INetServer _server;
        private Task _serverTask;

        public async Task InitializeAsync()
        {
            this._server = new TestServer();
            this._serverTask = Task.Factory.StartNew(() =>
            {
                this._server.Start();
            });

            // Wait for running state
            while (!this._server.IsRunning)
                await Task.Delay(100);
        }

        public async Task DisposeAsync()
        {
            this._server.Stop();
            this._serverTask.Wait();
        }

        [Fact]
        public async void ConnectToServer()
        {
            var client = new MyClient("127.0.0.1", 4444, 512);

            client.Connect();

            int attemps = 0;
            while (!client.IsConnected || attemps < 5)
            {
                await Task.Delay(1000);
                attemps++;
            }

            Assert.Equal(true, client.IsConnected);
        }
    }
}
