using Ether.Network.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ether.Network.Packets
{
    public class NetPacket : IDisposable
    {
        private MemoryStream memoryStream;
        private BinaryReader memoryReader;
        private BinaryWriter memoryWriter;
        private PacketStateType state;

        /// <summary>
        /// Gets the Packet buffer.
        /// </summary>
        public byte[] Buffer
        {
            get
            {
                if (this.state == PacketStateType.Write)
                {
                    this.memoryWriter.Seek(0, SeekOrigin.Begin);
                    this.Write(this.Size);
                    this.memoryWriter.Seek(this.Size, SeekOrigin.Begin);
                }

                return this.GetBuffer();
            }
        }

        /// <summary>
        /// Gets the size of the packet.
        /// </summary>
        public int Size
        {
            get { return (int)this.memoryStream.Length; }
        }

        /// <summary>
        /// Gets or sets the read/write position in the packet.
        /// </summary>
        public long Position
        {
            get { return (int)this.memoryStream.Position; }
            set { this.memoryStream.Position = value; }
        }

        /// <summary>
        /// Creates a new NetPacket in write-only mode.
        /// </summary>
        public NetPacket()
        {
            this.state = PacketStateType.Write;

            this.memoryStream = new MemoryStream();
            this.memoryWriter = new BinaryWriter(this.memoryStream);
            this.Write(0); // packet size
        }

        /// <summary>
        /// Creates a new NetPacket in read-only mode.
        /// </summary>
        /// <param name="buffer"></param>
        public NetPacket(byte[] buffer)
        {
            this.state = PacketStateType.Read;

            this.memoryStream = new MemoryStream(buffer, 0, buffer.Length);
            this.memoryReader = new BinaryReader(this.memoryStream);
        }

        /// <summary>
        /// Gets the packet buffer.
        /// </summary>
        /// <returns></returns>
        protected byte[] GetBuffer()
        {
            ArraySegment<byte> buffer;

            this.memoryStream.TryGetBuffer(out buffer);

            return buffer.ToArray();
        }

        /// <summary>
        /// Writes a T value in the packet.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="value">Value.</param>
        public void Write<T>(T value)
        {
            if (this.state != PacketStateType.Write)
                throw new InvalidOperationException("Packet is in read-only mode.");

            var type = typeof(T);

            if (NetPacketMethods.WriteMethods.ContainsKey(type))
                NetPacketMethods.WriteMethods[type](this.memoryWriter, value);
        }

        /// <summary>
        /// Reads a T value from the packet.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <returns>Value.</returns>
        public T Read<T>()
        {
            if (this.state != PacketStateType.Read)
                throw new InvalidOperationException("Packet is in write-only mode.");

            var type = typeof(T);

            if (NetPacketMethods.ReadMethods.ContainsKey(type))
                return (T)NetPacketMethods.ReadMethods[type](this.memoryReader);

            return default(T);
        }

        /// <summary>
        /// Dispose the NetPacket resources.
        /// </summary>
        public void Dispose()
        {
            if (this.memoryReader != null)
            {
                this.memoryReader.Dispose();
                this.memoryReader = null;
            }

            if (this.memoryWriter != null)
            {
                this.memoryWriter.Dispose();
                this.memoryWriter = null;
            }

            if (this.memoryStream != null)
            {
                this.memoryStream.Dispose();
                this.memoryStream = null;
            }
        }

        /// <summary>
        /// Split the incoming packets.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static NetPacket[] Split(byte[] buffer)
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

            return packets.ToArray();
        }
    }
}
