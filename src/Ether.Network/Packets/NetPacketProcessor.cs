using Ether.Network.Core;
using System.IO;

namespace Ether.Network.Packets
{
    internal sealed class NetPacketProcessor : IPacketProcessor<NetPacket>
    {
        /// <summary>
        /// Gets the <see cref="NetPacket"/> header size.
        /// </summary>
        public int HeaderSize => 4;

        /// <summary>
        /// Gets the <see cref="NetPacket"/> length size.
        /// </summary>
        /// <param name="buffer">Incoming buffer</param>
        /// <returns>Packet length</returns>
        public int GetLength(byte[] buffer)
        {
            int packetLength = 0;

            using (var memoryStream = new MemoryStream(buffer))
            using (var binaryReader = new BinaryReader(memoryStream))
                packetLength = binaryReader.ReadInt32();

            return packetLength;
        }

        /// <summary>
        /// Creates a <see cref="NetPacket"/> instance.
        /// </summary>
        /// <param name="buffer">Input buffer</param>
        /// <returns></returns>
        public NetPacket CreatePacket(byte[] buffer)
        {
            return new NetPacket(buffer);
        }
    }
}
