using Ether.Network.Interfaces;
using System;
using System.Net.Sockets;

namespace Ether.Network
{
    /// <summary>
    /// Represents a network connection.
    /// </summary>
    public abstract class NetConnection : INetConnection, IDisposable
    {
        private bool _disposedValue = false;

        /// <summary>
        /// Gets or sets the connection's Id.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets or sets the connection's socket.
        /// </summary>
        public Socket Socket { get; internal set; }

        /// <summary>
        /// Creates a new <see cref="NetConnection"/> instance.
        /// </summary>
        protected NetConnection()
        {
            this.Id = Guid.NewGuid();
        }

        /// <summary>
        /// Disposes the current <see cref="NetConnection"/> resources.
        /// </summary>
        ~NetConnection()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Disposes the current <see cref="NetConnection"/> resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the current <see cref="NetConnection"/> resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    this.Socket.Dispose();
                }

                _disposedValue = true;
            }
        }
    }
}
