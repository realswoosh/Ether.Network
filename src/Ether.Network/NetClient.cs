using Ether.Network.Packets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Ether.Network
{
    /// <summary>
    /// NetClient class.
    /// </summary>
    public abstract class NetClient : NetConnection
    {
        private bool isRunning;

        /// <summary>
        /// Creates a new NetClient instance.
        /// </summary>
        public NetClient()
            : base(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
            this.isRunning = false;
        }

        /// <summary>
        /// Connect to a remote server.
        /// </summary>
        /// <param name="ip">Server ip address</param>
        /// <param name="port">Server port</param>
        public void Connect(string ip, int port)
        {
            if (this.Socket.Connected)
                return;

            this.Socket.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
            this.isRunning = true;
        }

        /// <summary>
        /// Disconnect the client from the server.
        /// </summary>
        public void Disconnect()
        {
            if (this.Socket.Connected)
                this.Socket.Dispose();
        }

        /// <summary>
        /// Run the client.
        /// </summary>
        public void Run()
        {
            while (this.isRunning)
            {
                if (!this.Socket.Poll(100, SelectMode.SelectRead))
                    continue;

                try
                {
                    var buffer = new byte[this.Socket.Available];
                    var recievedDataSize = this.Socket.Receive(buffer);

                    if (recievedDataSize < 0)
                        throw new Exception("Disconnected");
                    else
                    {
                        var recievedPackets = NetPacket.Split(buffer);

                        foreach (var packet in recievedPackets)
                        {
                            this.HandleMessage(packet);
                            packet.Dispose();
                        }
                    }
                }
                catch (Exception e)
                {
                    if (this.Socket.Connected == false)
                    {
                        this.OnClientDisconnected();
                        this.isRunning = false;
                    }
                    else
                        Console.WriteLine("Error: {0}", e.Message);
                }
            }
        }

        public override void Greetings() { }

        public override void HandleMessage(NetPacketBase packet) { }

        protected abstract void OnClientDisconnected();

        /// <summary>
        /// Split a buffer into packets.
        /// </summary>
        /// <param name="buffer">Incoming buffer</param>
        /// <returns></returns>
        protected virtual IReadOnlyCollection<NetPacketBase> SplitPackets(byte[] buffer)
        {
            return NetPacket.Split(buffer);
        }
    }
}
