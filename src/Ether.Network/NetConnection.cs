using Ether.Network.Packets;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Ether.Network
{
    /// <summary>
    /// Net connection representing a connection.
    /// </summary>
    public abstract class NetConnection : IDisposable
    {
        private int _bufferSize;
        private int _bufferOffset;

        /// <summary>
        /// Gets the generated unique Id of the connection.
        /// </summary>
        public Guid Id { get; protected set; }

        /// <summary>
        /// Gets the connection socket.
        /// </summary>
        public Socket Socket { get; protected set; }
        
        /// <summary>
        /// Creates a new <see cref="NetConnection"/> instance.
        /// </summary>
        protected NetConnection()
        {
        }
        
        internal void Initialize(Socket socket, SocketAsyncEventArgs e, int bufferSize)
        {
            this.Socket = socket;
            this._bufferSize = bufferSize;
        }

        /// <summary>
        /// Send welcome packet to client.
        /// </summary>
        public abstract void Greetings();

        /// <summary>
        /// Handle packets.
        /// </summary>
        /// <param name="packet">Packet recieved.</param>
        public abstract void HandleMessage(NetPacketBase packet);

        /// <summary>
        /// Send a packet to this client.
        /// </summary>
        /// <param name="packet"></param>
        public void Send(NetPacketBase packet)
        {
            this.Socket.Send(packet.Buffer);
        }

        /// <summary>
        /// Send a packet to the client passed as parameter.
        /// </summary>
        /// <param name="destClient">Destination client</param>
        /// <param name="packet">Packet to send</param>
        public static void SendTo(NetConnection destClient, NetPacketBase packet)
        {
            destClient.Send(packet);
        }

        /// <summary>
        /// Send to a collection of clients.
        /// </summary>
        /// <param name="clients">Clients</param>
        /// <param name="packet">Packet to send</param>
        public static void SendTo(ICollection<NetConnection> clients, NetPacketBase packet)
        {
            foreach (var client in clients)
                client.Send(packet);
        }

        /// <summary>
        /// Dispose the NetConnection resources.
        /// </summary>
        public void Dispose()
        {
            if (this.Socket == null)
                return;

            this.Socket.Dispose();
            this.Socket = null;
        }
    }
}
