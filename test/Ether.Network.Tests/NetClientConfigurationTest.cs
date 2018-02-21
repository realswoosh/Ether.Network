using System;
using Ether.Network.Exceptions;
using Ether.Network.Tests.Contexts.NetConfig;
using Xunit;

namespace Ether.Network.Tests
{
    public class NetClientConfigurationTest
    {
        [Fact]
        public void StartClientWihtoutConfiguration()
        {
            using (var server = new ConfigServer())
            {
                server.SetupConfiguration();
                server.Start();

                using (var client = new ConfigClient())
                {
                    Exception ex = Assert.Throws<EtherConfigurationException>(() => client.Connect());
                    Assert.IsType<EtherConfigurationException>(ex);

                    client.Disconnect();
                }
            }
        }

        [Fact]
        public void StartClientWithConfiguration()
        {
            using (var server = new ConfigServer())
            {
                server.SetupConfiguration();
                server.Start();

                using (var client = new ConfigClient())
                {
                    client.SetupConfiguration();
                    client.Connect();
                    client.Disconnect();
                }
            }
        }

        [Fact]
        public void SetupClientConfigurationAfterConnected()
        {
            using (var server = new ConfigServer())
            {
                server.SetupConfiguration();
                server.Start();

                using (var client = new ConfigClient())
                {
                    client.SetupConfiguration();
                    client.Connect();

                    Exception ex = Assert.Throws<EtherConfigurationException>(() => client.SetupConfiguration());
                    Assert.IsType<EtherConfigurationException>(ex);

                    client.Disconnect();
                }
            }
        }
    }
}