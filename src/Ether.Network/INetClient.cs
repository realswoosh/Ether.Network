using System;
using Ether.Network.Packets;

namespace Ether.Network
{
    /// <summary>
    /// <see cref="INetClient"/> interface.
    /// </summary>
    public interface INetClient
    {
        /// <summary>
        /// Gets the <see cref="INetClient"/> unique Id.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the <see cref="INetClient"/> connected state.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Connects to a remote server.
        /// </summary>
        void Connect();
        
        /// <summary>
        /// Disconnects from the remote server.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Sends packets to the remote server.
        /// </summary>
        /// <param name="packet"></param>
        void Send(NetPacketBase packet);
    }
}
