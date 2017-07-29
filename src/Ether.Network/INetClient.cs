using System;
using Ether.Network.Packets;

namespace Ether.Network
{
    public interface INetClient
    {
        Guid Id { get; }
        bool IsConnected { get; }
        void Connect();
        void Disconnect();
        void Send(NetPacketBase packet);
    }
}
