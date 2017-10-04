using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Threading.Tasks;
using System.Threading;
using Ether.Network.Exceptions;
using System;

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

        private async Task<bool> DelayAction(int attempts, int milliseconds, Func<bool> func)
        {
            int currentAttempt = 0;

            while (func.Invoke() || currentAttempt < attempts)
            {
                await Task.Delay(milliseconds);
                currentAttempt++;
            }

            return true;
        }

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


        }
    }
}
