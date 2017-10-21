using Ether.Network.Core;
using System;
using System.IO;
using System.Linq;

namespace Ether.Network.Packets
{
    public class NetPacketStream : MemoryStream, INetPacketStream
    {
        private readonly PacketStateType _state;
        private readonly BinaryReader _memoryReader;
        private readonly BinaryWriter _memoryWriter;

        /// <summary>
        /// Gets the <see cref="NetPacketStream"/> size.
        /// </summary>
        public int Size => (int)this.Length;

        /// <summary>
        /// Gets the <see cref="NetPacketStream"/> buffer.
        /// </summary>
        public virtual byte[] Buffer => this.GetBuffer();

        /// <summary>
        /// Creates and initializes a new <see cref="NetPacketStream"/> instance in write-only mode.
        /// </summary>
        public NetPacketStream()
        {
            this._memoryWriter = new BinaryWriter(this);
            this._state = PacketStateType.Write;
        }

        /// <summary>
        /// Creates and initializes a new <see cref="NetPacketStream"/> instance in read-only mode.
        /// </summary>
        /// <param name="buffer">Input buffer</param>
        public NetPacketStream(byte[] buffer)
            : base(buffer)
        {
            this._memoryReader = new BinaryReader(this);
            this._state = PacketStateType.Read;
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
                return (T)NetPacketMethods.ReadMethods[type](this._memoryReader);

            return default(T);
        }

        /// <summary>
        /// Reads an array of T value from the packet.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="amount">Amount to read.</param>
        /// <returns></returns>
        public virtual T[] Read<T>(int amount)
        {
            if (this._state != PacketStateType.Read)
                throw new InvalidOperationException("Packet is in write-only mode.");

            var array = new T[amount];
            var type = typeof(T);

            if (type == typeof(byte))
                array = this._memoryReader.ReadBytes(amount) as T[];
            else if (NetPacketMethods.ReadMethods.ContainsKey(type))
            {
                for (int i = 0; i < amount; i++)
                    array[i] = this.Read<T>();
            }

            return array;
        }

        /// <summary>
        /// Writes a T value in the packet.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="value">Value.</param>
        public new void Write<T>(T value)
        {
            if (this._state != PacketStateType.Write)
                throw new InvalidOperationException("Packet is in read-only mode.");

            var type = typeof(T);

            if (NetPacketMethods.WriteMethods.ContainsKey(type))
                NetPacketMethods.WriteMethods[type](this._memoryWriter, value);
        }

        private new byte[] GetBuffer()
        {
#if NET45 || NET451
            return base.GetBuffer();
#else
            if (this.TryGetBuffer(out ArraySegment<byte> buffer))
                return buffer.ToArray();

            return new byte[0];
#endif
        }

        /// <summary>
        /// Dispose the <see cref="NetPacketStream"/> internal resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            //if (disposing)
            //{
            //    this._memoryReader?.Dispose();
            //    this._memoryWriter?.Dispose();
            //}

            base.Dispose(disposing);
        }
    }
}
