using System;

namespace Ether.Network.Tests.Context
{
    internal class TestServer : NetServer<TestClient>
    {
        public TestServer()
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
}
