using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace Ether.Network
{
    internal sealed class SocketAsyncEventArgsPool
    {
        private int _nextTokenId = 0;
        private Stack<SocketAsyncEventArgs> _pool;
        
        /// <summary>
        /// Initialize the object pool to the specified size.
        /// </summary>
        /// <param name="capacity">Maximum number of <see cref="SocketAsyncEventArgs"/>.</param>
        internal SocketAsyncEventArgsPool(int capacity)
        {
            this._pool = new Stack<SocketAsyncEventArgs>(capacity);
        }

        /// <summary>
        /// Gets the number of <see cref="SocketAsyncEventArgs"/> in the pool.
        /// </summary>
        internal int Count => this._pool.Count;

        internal int AssignTokenId()
        {
            Int32 tokenId = Interlocked.Increment(ref _nextTokenId);

            return tokenId;
        }

        // Removes a SocketAsyncEventArgs instance from the pool.
        // returns SocketAsyncEventArgs removed from the pool.
        internal SocketAsyncEventArgs Pop()
        {
            lock (this._pool)
            {
                return this._pool.Pop();
            }
        }

        // Add a SocketAsyncEventArg instance to the pool.
        // "item" = SocketAsyncEventArgs instance to add to the pool.
        internal void Push(SocketAsyncEventArgs item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null");
            }

            lock (this._pool)
            {
                this._pool.Push(item);
            }
        }
    }
}
