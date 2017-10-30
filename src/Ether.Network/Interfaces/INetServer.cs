using System;

namespace Ether.Network.Interfaces
{
    /// <summary>
    /// NetServer interface.
    /// </summary>
    public interface INetServer : IDisposable
    {
        /// <summary>
        /// Gets the <see cref="INetServer"/> running state.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Start the server.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop the server.
        /// </summary>
        void Stop();

        /// <summary>
        /// Disconnects a client from the server.
        /// </summary>
        /// <param name="clientId">Client unique id</param>
        void DisconnectClient(Guid clientId);
    }
}
