using Ether.Network.Core;
using System;
using System.IO;

namespace Ether.Network.Packets
{
    internal sealed class NetPacketProcessor : IPacketProcessor
    {
        public int HeaderSize => 4;

        public int GetLength(byte[] buffer)
        {
            int packetLength = 0;

            using (var memoryStream = new MemoryStream(buffer))
            {
                using (var binaryReader = new BinaryReader(memoryStream))
                {
                    packetLength = binaryReader.ReadInt32();
                }
            }

            return packetLength;
        }
    }
}
