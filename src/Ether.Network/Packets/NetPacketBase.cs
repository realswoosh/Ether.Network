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
        private PacketStateType _state;

        /// <summary>
        /// Packet memory stream.
        /// </summary>
        protected MemoryStream MemoryStream { get; private set; }

        /// <summary>
        /// Packet memory reader.
        /// </summary>
        protected BinaryReader MemoryReader { get; private set; }

        /// <summary>
        /// Packet memory writer.
        /// </summary>
        protected BinaryWriter MemoryWriter { get; private set; }

        /// <summary>
        /// Gets the packet buffer.
        /// </summary>
        public abstract byte[] Buffer { get; }

        /// <summary>
        /// Gets the size of the packet.
        /// </summary>
        public int Size => (int)this.MemoryStream.Length;

        /// <summary>
        /// Gets or sets the read/write position in the packet.
        /// </summary>
        public long Position
        {
            get { return (int)this.MemoryStream.Position; }
            set { this.MemoryStream.Position = value; }
        }

        /// <summary>
        /// Creates a new NetPacketBase in write-only mode.
        /// </summary>
        protected NetPacketBase()
        {
            this._state = PacketStateType.Write;

            this.MemoryStream = new MemoryStream();
            this.MemoryWriter = new BinaryWriter(this.MemoryStream);
        }

        /// <summary>
        /// Creates a new NetPacketBase in read-only mode.
        /// </summary>
        /// <param name="buffer"></param>
        protected NetPacketBase(byte[] buffer)
        {
            this._state = PacketStateType.Read;

            this.MemoryStream = new MemoryStream(buffer, 0, buffer.Length, false, true);
            this.MemoryReader = new BinaryReader(this.MemoryStream);
        }

        /// <summary>
        /// Gets the packet buffer.
        /// </summary>
        /// <returns></returns>
        protected byte[] GetBuffer()
        {
#if NET45 || NET451
            return this.MemoryStream.GetBuffer();
#else
            this.MemoryStream.TryGetBuffer(out ArraySegment<byte> buffer);
            return buffer.ToArray();
#endif
        }

        /// <summary>
        /// Writes a T value in the packet.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="value">Value.</param>
        public virtual void Write<T>(T value)
        {
            if (this._state != PacketStateType.Write)
                throw new InvalidOperationException("Packet is in read-only mode.");

            var type = typeof(T);

            if (NetPacketMethods.WriteMethods.ContainsKey(type))
                NetPacketMethods.WriteMethods[type](this.MemoryWriter, value);
        }

        /// <summary>
        /// Reads a T value from the packet.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <returns>Value.</returns>
        public virtual T Read<T>()
        {
            if (this._state != PacketStateType.Read)
                throw new InvalidOperationException("Packet is in write-only mode.");

            var type = typeof(T);

            if (NetPacketMethods.ReadMethods.ContainsKey(type))
                return (T)NetPacketMethods.ReadMethods[type](this.MemoryReader);

            return default(T);
        }

        /// <summary>
        /// Dispose the NetPacketBase resources.
        /// </summary>
        public void Dispose()
        {
            if (this.MemoryReader != null)
            {
                this.MemoryReader.Dispose();
                this.MemoryReader = null;
            }

            if (this.MemoryWriter != null)
            {
                this.MemoryWriter.Dispose();
                this.MemoryWriter = null;
            }

            if (this.MemoryStream != null)
            {
                this.MemoryStream.Dispose();
                this.MemoryStream = null;
            }
        }
    }
}
