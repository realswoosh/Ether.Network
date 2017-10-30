using System;
using System.Net.Sockets;

namespace Ether.Network.Interfaces
{
    public interface INetConnection
    {
        Guid Id { get; }

        Socket Socket { get; }
    }
}
