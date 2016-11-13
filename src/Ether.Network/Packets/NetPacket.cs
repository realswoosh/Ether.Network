using Ether.Network.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ether.Network.Packets
{
    public class NetPacket : NetPacketBase
    {
        /// <summary>
        /// Gets the packet buffer.
        /// </summary>
        public override byte[] Buffer
        {
            get
            {
                this.memoryWriter.Seek(0, SeekOrigin.Begin);
                this.Write(this.Size);
                this.memoryWriter.Seek(this.Size, SeekOrigin.Begin);

                return this.GetBuffer();
            }
        }

        /// <summary>
        /// Creates a new NetPacket in write-only mode.
        /// </summary>
        public NetPacket()
            : base()
        {
            this.Write(0); // Packet size
        }

        /// <summary>
        /// Creates a new NetPacket in read-only mode.
        /// </summary>
        /// <param name="buffer"></param>
        public NetPacket(byte[] buffer)
            : base(buffer)
        {
        }

        /// <summary>
        /// Split the incoming packets.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static IReadOnlyCollection<NetPacketBase> Split(byte[] buffer)
        {
            var packets = new List<NetPacket>();

            using (var memoryStream = new MemoryStream(buffer))
            using (var readerStream = new BinaryReader(memoryStream))
            {
                try
                {
                    while (readerStream.BaseStream.Position < readerStream.BaseStream.Length)
                    {
                        var packetSize = readerStream.ReadInt32();

                        if (packetSize == 0)
                            break;
                        
                        packets.Add(new NetPacket(readerStream.ReadBytes(packetSize)));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occured during the packet spliting. {0}", e.Message);
                }
            }

            return packets;
        }
    }
}
