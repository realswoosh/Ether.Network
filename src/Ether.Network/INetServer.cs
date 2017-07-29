using Ether.Network.Packets;
using System.Collections.Generic;

namespace Ether.Network
{
    public interface INetServer
    {
        bool IsRunning { get; }
        void Start();
        void Stop();

        IReadOnlyCollection<NetPacketBase> SplitPackets(byte[] buffer);
    }
}
