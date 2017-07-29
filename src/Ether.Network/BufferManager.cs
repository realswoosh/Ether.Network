using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Ether.Network
{
    public class BufferManager
    {
        private readonly int _bufferSize;
        private readonly byte[] _buffer;
        private readonly ConcurrentStack<int> _freeIndexPool;
        private int _currentIndex;

        public BufferManager(int capacity, int bufferSize)
        {
            this._bufferSize = bufferSize;
            this._buffer = new byte[capacity * bufferSize];
            this._freeIndexPool = new ConcurrentStack<int>();
        }

        public void SetBuffer(SocketAsyncEventArgs e)
        {
            if (this._freeIndexPool.Count > 0 && this._freeIndexPool.TryPop(out int offset))
                e.SetBuffer(this._buffer, offset, this._bufferSize);
            else
            {
                e.SetBuffer(this._buffer, this._currentIndex, this._bufferSize);
                this._currentIndex += this._bufferSize;
            }
        }

        public void FreeBuffer(SocketAsyncEventArgs e)
        {
            this._freeIndexPool.Push(e.Offset);
            e.SetBuffer(this._buffer, 0, 0);
        }
    }
}
