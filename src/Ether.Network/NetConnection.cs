using Ether.Network.Packets;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Ether.Network
{
    /// <summary>
    /// Net connection representing a connection.
    /// </summary>
    public class NetConnection : IDisposable
    {
        /// <summary>
        /// Gets the generated unique Id of the connection.
        /// </summary>
        public Guid Id { get; protected set; }

        /// <summary>
        /// Gets the connection socket.
        /// </summary>
        public Socket Socket { get; protected set; }
        
        /// <summary>
        /// Creates a new NetConnection instance.
        /// </summary>
        public NetConnection()
            : this(null)
        {
        }

        /// <summary>
        /// Creates a new NetConnection instance with a socket.
        /// </summary>
        /// <param name="acceptedSocket">Client socket.</param>
        public NetConnection(Socket acceptedSocket)
        {
            this.Id = Guid.NewGuid();
            this.Initialize(acceptedSocket);
        }

        /// <summary>
        /// Initialize the socket and send greetings to the client to inform him that he's connected.
        /// </summary>
        /// <param name="acceptedSocket">Client socket.</param>
        internal void Initialize(Socket acceptedSocket)
        {
            if (this.Socket != null)
                return;

            this.Socket = acceptedSocket;
            this.Greetings();
        }

        /// <summary>
        /// Send welcome packet to client.
        /// </summary>
        public virtual void Greetings() { }

        /// <summary>
        /// Handle packets.
        /// </summary>
        /// <param name="packet">Packet recieved.</param>
        public virtual void HandleMessage(NetPacketBase packet) { }

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
        public void SendTo(NetConnection destClient, NetPacketBase packet)
        {
            destClient.Send(packet);
        }

        /// <summary>
        /// Send to a collection of clients.
        /// </summary>
        /// <param name="clients">Clients</param>
        /// <param name="packet">Packet to send</param>
        public void SendTo(ICollection<NetConnection> clients, NetPacketBase packet)
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
