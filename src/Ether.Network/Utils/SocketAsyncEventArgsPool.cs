using System;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Ether.Network.Utils
{
    /// <summary>
    /// Represents an object pool of <see cref="SocketAsyncEventArgs"/> elements.
    /// </summary>
    internal sealed class SocketAsyncEventArgsPool : IDisposable
    {
        private readonly ConcurrentStack<SocketAsyncEventArgs> _socketPool;

        /// <summary>
        /// Creates a new <see cref="SocketAsyncEventArgsPool"/> instance with a maximal capacity.
        /// </summary>
        /// <param name="capacity">Maximal capacity</param>
        public SocketAsyncEventArgsPool(int capacity)
        {
            this._socketPool = new ConcurrentStack<SocketAsyncEventArgs>();

            for (int i = 0; i < capacity; i++)
                this._socketPool.Push(new SocketAsyncEventArgs());
        }

        /// <summary>
        /// Pops a <see cref="SocketAsyncEventArgs"/> of the top of the stack.
        /// </summary>
        /// <returns></returns>
        public SocketAsyncEventArgs Pop()
        {
            if (this._socketPool.TryPop(out SocketAsyncEventArgs socketAsyncEventArgs))
                return socketAsyncEventArgs;
            return null;
        }

        /// <summary>
        /// Push a <see cref="SocketAsyncEventArgs"/> to the top of the stack.
        /// </summary>
        /// <param name="socketAsyncEventArgs"></param>
        public void Push(SocketAsyncEventArgs socketAsyncEventArgs)
        {
            this._socketPool.Push(socketAsyncEventArgs);
        }

        #region IDisposable Support

        private bool _disposedValue;

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    foreach (SocketAsyncEventArgs e in this._socketPool)
                        e.Dispose();

                    this._socketPool.Clear();
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
