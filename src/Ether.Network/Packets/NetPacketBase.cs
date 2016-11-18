using Ether.Network.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ether.Network.Packets
{
    /// <summary>
    /// NetPacketBase provides all methods to manage a packet at the memory level.
    /// </summary>
    public abstract class NetPacketBase : IDisposable
    {
        private PacketStateType state;

        /// <summary>
        /// Packet memory stream.
        /// </summary>
        protected MemoryStream memoryStream;

        /// <summary>
        /// Packet memory reader.
        /// </summary>
        protected BinaryReader memoryReader;

        /// <summary>
        /// Packet memory writer.
        /// </summary>
        protected BinaryWriter memoryWriter;

        /// <summary>
        /// Gets the packet buffer.
        /// </summary>
        public abstract byte[] Buffer { get; }

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
        /// Creates a new NetPacketBase in write-only mode.
        /// </summary>
        public NetPacketBase()
        {
            this.state = PacketStateType.Write;

            this.memoryStream = new MemoryStream();
            this.memoryWriter = new BinaryWriter(this.memoryStream);
        }

        /// <summary>
        /// Creates a new NetPacketBase in read-only mode.
        /// </summary>
        /// <param name="buffer"></param>
        public NetPacketBase(byte[] buffer)
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
        public virtual void Write<T>(T value)
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
        public virtual T Read<T>()
        {
            if (this.state != PacketStateType.Read)
                throw new InvalidOperationException("Packet is in write-only mode.");

            var type = typeof(T);

            if (NetPacketMethods.ReadMethods.ContainsKey(type))
                return (T)NetPacketMethods.ReadMethods[type](this.memoryReader);

            return default(T);
        }

        /// <summary>
        /// Dispose the NetPacketBase resources.
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
    }
}
