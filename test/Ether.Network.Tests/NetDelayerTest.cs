using System;
using Xunit;

namespace Ether.Network.Tests
{
    public class NetDelayerTest
    {
        [Fact]
        public void NetDelayerRegisterSuccess()
        {
            int actionCount = 0;

            NetDelayer.Start();
            NetDelayer.Register(() => Console.WriteLine("My Action"));
            actionCount = NetDelayer.ActionCount;

            NetDelayer.Stop();

            Assert.Equal(1, actionCount);
        }

        [Fact]
        public void NetDelayerUnregisterSuccess()
        {
            int actionCount = 0;

            NetDelayer.Start();
            // Register the action
            int actionId = NetDelayer.Register(() => Console.WriteLine("My Action"));

            // Unregister the action
            NetDelayer.Unregister(actionId);

            // Get the number of actions
            actionCount = NetDelayer.ActionCount;

            NetDelayer.Stop();

            Assert.Equal(0, actionCount);
        }
    }
}
