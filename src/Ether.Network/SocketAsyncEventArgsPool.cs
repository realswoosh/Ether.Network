using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Ether.Network
{
    public sealed class SocketAsyncEventArgsPool : IDisposable
    {
        private readonly ConcurrentStack<SocketAsyncEventArgs> _socketPool;

        public SocketAsyncEventArgsPool(int capacity)
        {
            this._socketPool = new ConcurrentStack<SocketAsyncEventArgs>();

            for (int i = 0; i < capacity; i++)
                this._socketPool.Push(new SocketAsyncEventArgs());
        }

        public SocketAsyncEventArgs Pop()
        {
            if (this._socketPool.TryPop(out SocketAsyncEventArgs socketAsyncEventArgs))
                return socketAsyncEventArgs;
            return null;
        }

        public void Push(SocketAsyncEventArgs socketAsyncEventArgs)
        {
            this._socketPool.Push(socketAsyncEventArgs);
        }

        #region IDisposable Support

        private bool _disposedValue;

        void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    foreach (SocketAsyncEventArgs e in this._socketPool)
                        e.Dispose();
                }
                
                _disposedValue = true;
            }
        }
        
        ~SocketAsyncEventArgsPool()
        {
            this.Dispose(false);
        }
        
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
