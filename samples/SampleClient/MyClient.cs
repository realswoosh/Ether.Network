using Ether.Network;
using System;
using Ether.Network.Packets;
using System.Net.Sockets;
using Ether.Network.Client;
using Ether.Network.Core;

namespace SampleClient
{
    class MyPacket : NetPacketStream
    {
        public MyPacket()
        {
        }

        public MyPacket(byte[] buffer)
            : base(buffer)
        {
        }
    }

    class PacketProcessor : IPacketProcessor
    {
        public int HeaderSize => throw new NotImplementedException();

        public INetPacketStream CreatePacket(byte[] buffer)
        {
            return new MyPacket();
        }

        public int GetLength(byte[] buffer)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class MyClient : NetClient
    {
        protected override IPacketProcessor PacketProcessor => new PacketProcessor();

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
        protected override void HandleMessage(INetPacketStream packet)
        {
            var response = packet.Read<string>();
            Console.WriteLine($"-> Server response: '{response}'");

            string pak = $"Request #{i++}";

            using (var newPacket = new NetPacket())
            {
                newPacket.Write(pak);

                this.Send(newPacket);
            }

        }
        static int i = 0;

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