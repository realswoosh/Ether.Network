using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace Ether.Network.Utils
{
    /// <summary>
    /// Buffer manager.
    /// </summary>
    internal sealed class BufferManager
    {
        private readonly int _bufferSize;
        private readonly byte[] _buffer;
        private readonly ConcurrentStack<int> _freeIndexPool;
        private int _currentIndex;

        /// <summary>
        /// Creates a new <see cref="BufferManager"/> instance.
        /// </summary>
        /// <param name="capacity">Maximal capacity</param>
        /// <param name="bufferSize">Buffer size per item</param>
        public BufferManager(int capacity, int bufferSize)
        {
            this._bufferSize = bufferSize;
            this._buffer = new byte[capacity * bufferSize];
            this._freeIndexPool = new ConcurrentStack<int>();
        }

        /// <summary>
        /// Sets the buffer for a <see cref="SocketAsyncEventArgs"/>.
        /// </summary>
        /// <param name="e"></param>
        public void SetBuffer(SocketAsyncEventArgs e)
        {
            if (this._freeIndexPool.Count > 0 && this._freeIndexPool.TryPop(out int offset))
                e.SetBuffer(this._buffer, offset, this._bufferSize);
            else
            {
                if ((this._buffer.Length - this._bufferSize) < this._currentIndex)
                    return;

                e.SetBuffer(this._buffer, this._currentIndex, this._bufferSize);
                this._currentIndex += this._bufferSize;
            }
        }

        /// <summary>
        /// Releases the buffer for a <see cref="SocketAsyncEventArgs"/>.
        /// </summary>
        /// <param name="e"></param>
        public void FreeBuffer(SocketAsyncEventArgs e)
        {
            this._freeIndexPool.Push(e.Offset);
            e.SetBuffer(null, 0, 0);
        }
    }
}
