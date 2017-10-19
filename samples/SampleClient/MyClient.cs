using Ether.Network;
using System;
using Ether.Network.Packets;
using System.Net.Sockets;
using Ether.Network.Client;

namespace SampleClient
{
    internal sealed class MyClient : NetClient
    {
        /// <summary>
        /// Creates a new <see cref="MyClient"/>.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="bufferSize"></param>
        public MyClient(string host, int port, int bufferSize) 
            : base(host, port, bufferSize)
        {
        }

        /// <summary>
        /// Handles incoming messages.
        /// </summary>
        /// <param name="packet"></param>
        protected override void HandleMessage(NetPacketBase packet)
        {
            var response = packet.Read<string>();
            Console.WriteLine("-> Server response: {0}", response);

            using (var newPacket = new NetPacket())
            {
                newPacket.Write(response);

                this.Send(newPacket);
            }

        }

        /// <summary>
        /// Triggered when connected to the server.
        /// </summary>
        protected override void OnConnected()
        {
            Console.WriteLine("Connected to {0}", this.Socket.RemoteEndPoint.ToString());
        }

        /// <summary>
        /// Triggered when disconnected from the server.
        /// </summary>
        protected override void OnDisconnected()
        {
            Console.WriteLine("Disconnected");
        }

        protected override void OnSocketError(SocketError socketError)
        {
            Console.WriteLine("Socket Error: {0}", socketError.ToString());
        }
    }
}