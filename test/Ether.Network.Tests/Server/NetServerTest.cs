using System;
using System.Collections.Generic;
using System.Text;
using Ether.Network.Packets;
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
            this._server = new MyServer();
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

    internal class MyServer : NetServer<TestClient>
    {
        public MyServer()
        {
            this.Configuration.Host = "127.0.0.1";
            this.Configuration.Port = 4444;
            this.Configuration.MaximumNumberOfConnections = 100;
            this.Configuration.Backlog = 100;
            this.Configuration.BufferSize = 512;
        }

        protected override void Initialize()
        {
        }

        protected override void OnClientConnected(TestClient connection)
        {
            connection.SendHello();
        }

        protected override void OnClientDisconnected(TestClient connection)
        {
        }

        protected override void OnError(Exception exception)
        {
        }
    }

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

    internal class MyClient : NetClient
    {
        public MyClient(string host, int port, int bufferSize) 
            : base(host, port, bufferSize)
        {
        }

        protected override void HandleMessage(NetPacketBase packet)
        {
            var header = packet.Read<int>();

            switch (header)
            {
                case 0:
                    var message = packet.Read<string>();
                    Console.WriteLine("Received: {0}", message);
                    break;
            }
        }

        protected override void OnConnected()
        {
        }

        protected override void OnDisconnected()
        {
        }
    }
}
