using System;
using System.Linq;

namespace Ether.Network.Packets
{
    internal sealed class NetPacketProcessor : IPacketProcessor
    {
        /// <inheritdoc />
        public int HeaderSize => sizeof(int);

        /// <inheritdoc />
        public bool IncludeHeader => false;

        /// <inheritdoc />
        public int GetMessageLength(byte[] buffer) => BitConverter.ToInt32(buffer.Take(HeaderSize).ToArray(), 0);

        /// <inheritdoc />
        public INetPacketStream CreatePacket(byte[] buffer) => new NetPacket(buffer);
    }
}
