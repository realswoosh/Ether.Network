using Ether.Network.Core;
using System;
using System.IO;
using System.Linq;

namespace Ether.Network.Packets
{
    public class NetPacketStream : MemoryStream, INetPacketStream
    {
        private readonly PacketStateType _type;
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
            this._type = PacketStateType.Write;
        }

        /// <summary>
        /// Creates and initializes a new <see cref="NetPacketStream"/> instance in read-only mode.
        /// </summary>
        /// <param name="buffer">Input buffer</param>
        public NetPacketStream(byte[] buffer)
            : base(buffer)
        {
            this._memoryReader = new BinaryReader(this);
            this._type = PacketStateType.Read;
        }

        public new T Read<T>()
        {
            throw new NotImplementedException();
        }

        public new void Write<T>(T value)
        {
            throw new NotImplementedException();
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
            if (disposing)
            {
                this._memoryReader.Dispose();
                this._memoryWriter.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
