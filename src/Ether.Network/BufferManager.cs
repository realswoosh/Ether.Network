using System.Collections.Generic;
using System.Net.Sockets;

namespace Ether.Network
{
    public class BufferManager
    {
        private int _currentIndex;
        private int _capacity;
        private int _bufferSize;
        private int _totalBufferSize;
        private byte[] _buffer;
        private Stack<int> _freeIndexPool;

        public BufferManager(int capacity, int bufferSize)
        {
            this._capacity = capacity;
            this._bufferSize = bufferSize;
            this._totalBufferSize = this._capacity * this._bufferSize;
            this._buffer = new byte[this._totalBufferSize];
            this._freeIndexPool = new Stack<int>();
        }

        public void SetBuffer(SocketAsyncEventArgs e)
        {
            lock (this._freeIndexPool)
            {
                if (this._freeIndexPool.Count > 0)
                    e.SetBuffer(this._buffer, this._freeIndexPool.Pop(), this._bufferSize);
                else
                {
                    e.SetBuffer(this._buffer, this._currentIndex, this._bufferSize);
                    this._currentIndex += this._bufferSize;
                }
            }
        }

        public void FreeBuffer(SocketAsyncEventArgs e)
        {
            lock (this._freeIndexPool)
            {
                this._freeIndexPool.Push(e.Offset);
                e.SetBuffer(this._buffer, 0, 0);
            }
        }
    }
}
