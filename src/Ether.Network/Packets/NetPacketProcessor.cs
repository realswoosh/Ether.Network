using Ether.Network.Core;
using System.IO;

namespace Ether.Network.Packets
{
    internal sealed class NetPacketProcessor : IPacketProcessor
    {
        /// <summary>
        /// Gets the <see cref="NetPacket"/> header size.
        /// </summary>
        public int HeaderSize => sizeof(int);

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
        public INetPacketStream CreatePacket(byte[] buffer) => new NetPacket(buffer);
    }
}
