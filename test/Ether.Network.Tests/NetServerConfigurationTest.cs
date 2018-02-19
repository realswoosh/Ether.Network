using Ether.Network.Exceptions;
using Ether.Network.Tests.Contexts.NetConfig;
using System;
using Xunit;

namespace Ether.Network.Tests
{
    public class NetServerConfigurationTest
    {
        [Fact]
        public void StartServerWithoutConfiguration()
        {
            using (var server = new ConfigServer())
            {
                Exception ex = Assert.Throws<EtherConfigurationException>(() => server.Start());

                Assert.IsType<EtherConfigurationException>(ex);

                server.Stop();
            }
        }

        [Fact]
        public void SetupServerConfigurationBeforeStart()
        {
            using (var server = new ConfigServer())
            {
                server.SetupConfiguration();
                server.Start();
                server.Stop();
            }
        }

        [Fact]
        public void SetupServerConfigurationAfterStart()
        {
            using (var server = new ConfigServer())
            {
                server.SetupConfiguration();
                server.Start();

                Exception ex = Assert.Throws<EtherConfigurationException>(() => server.SetupConfiguration());

                Assert.IsType<EtherConfigurationException>(ex);

                server.Stop();
            }
        }

        [Fact]
        public void SetupServerConfigurationAfterStartStop()
        {
            using (var server = new ConfigServer())
            {
                server.SetupConfiguration();
                server.Start();
                server.Stop();

                server.SetupConfiguration();
                server.Start();
                server.Stop();
            }
        }
    }
}