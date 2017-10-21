using Ether.Network.Core;
using System.Threading.Tasks;
using Xunit;

namespace Ether.Network.Tests.Context
{
    public abstract class NetServerContext : IAsyncLifetime
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
    }
}
