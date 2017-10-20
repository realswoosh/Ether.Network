using Ether.Network.Core;
using Ether.Network.Exceptions;
using Ether.Network.Packets;
using Ether.Network.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Ether.Network.Client
{
    /// <summary>
    /// Managed TCP client.
    /// </summary>
    public abstract class NetClient : INetClient
    {
        private readonly static IPacketProcessor BasePacketProcessor = new NetPacketProcessor();

        private readonly Guid _id;
        private readonly string _host;
        private readonly int _port;
        private readonly IPEndPoint _ipEndPoint;
        private readonly Socket _socket;
        private readonly SocketAsyncEventArgs _socketReceiveArgs;
        private readonly SocketAsyncEventArgs _socketSendArgs;
        private readonly AutoResetEvent _autoConnectEvent;
        private readonly AutoResetEvent _autoSendEvent;
        private readonly BlockingCollection<NetPacketBase> _sendingQueue;
        private readonly BlockingCollection<byte[]> _receivingQueue;
        private readonly Task _sendingQueueWorker;
        private readonly Task _receivingQueueWorker;

        /// <summary>
        /// Gets the <see cref="NetClient"/> unique Id.
        /// </summary>
        public Guid Id => this._id;

        /// <summary>
        /// Gets the <see cref="NetClient"/> socket.
        /// </summary>
        protected Socket Socket => this._socket;

        /// <summary>
        /// Gets the packet processor.
        /// </summary>
        protected IPacketProcessor PacketProcessor => BasePacketProcessor;

        /// <summary>
        /// Gets the <see cref="NetClient"/> connected state.
        /// </summary>
        public bool IsConnected => this.Socket != null && this.Socket.Connected;

        /// <summary>
        /// Creates a new <see cref="NetClient"/> instance.
        /// </summary>
        /// <param name="host">Remote host or ip</param>
        /// <param name="port">Remote port</param>
        /// <param name="bufferSize">Buffer size</param>
        protected NetClient(string host, int port, int bufferSize)
        {
            this._id = Guid.NewGuid();
            this._host = host;
            this._port = port;
            this._ipEndPoint = NetUtils.CreateIpEndPoint(this._host, this._port);
            this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this._socketSendArgs = CreateSocketAsync(this.Socket, -1, this.IO_Completed);
            this._socketReceiveArgs = CreateSocketAsync(this.Socket, bufferSize, this.IO_Completed);
            this._socketReceiveArgs.UserToken = new AsyncUserToken(this._socket);
            this._autoConnectEvent = new AutoResetEvent(false);
            this._autoSendEvent = new AutoResetEvent(false);
            this._sendingQueue = new BlockingCollection<NetPacketBase>();
            this._receivingQueue = new BlockingCollection<byte[]>();
            this._sendingQueueWorker = new Task(this.ProcessSendingQueue);
            this._receivingQueueWorker = new Task(this.ProcessReceiveQueue);
        }

        /// <summary>
        /// Connect to the remote host.
        /// </summary>
        public void Connect()
        {
            if (this.IsConnected)
                throw new InvalidOperationException("Client is already connected to remote.");

            var connectSocket = new SocketAsyncEventArgs();
            connectSocket.RemoteEndPoint = this._ipEndPoint;
            connectSocket.UserToken = this._socket;
            connectSocket.Completed += this.IO_Completed;

            this._socket.ConnectAsync(connectSocket);
            this._autoConnectEvent.WaitOne();

            SocketError errorCode = connectSocket.SocketError;

            if (errorCode != SocketError.Success)
                throw new SocketException((Int32)errorCode);

            this._sendingQueueWorker.Start();
            this._receivingQueueWorker.Start();

            if (!this._socket.ReceiveAsync(this._socketReceiveArgs))
                this.ProcessReceive(this._socketReceiveArgs);
        }

        /// <summary>
        /// Disconnects the <see cref="NetClient"/>.
        /// </summary>
        public void Disconnect()
        {
            if (this.IsConnected)
            {
#if !NETSTANDARD1_3
                this._socket.Close();
#endif
                this._socket.Shutdown(SocketShutdown.Both);
                this._socket.Dispose();
            }
        }

        /// <summary>
        /// Sends a packet through the network.
        /// </summary>
        /// <param name="packet"></param>
        public void Send(NetPacketBase packet)
        {
            if (!this.IsConnected)
                throw new SocketException();

            this._sendingQueue.Add(packet);
        }

        /// <summary>
        /// Triggered when the <see cref="NetClient"/> receives a packet.
        /// </summary>
        /// <param name="packet"></param>
        protected abstract void HandleMessage(NetPacketBase packet);

        /// <summary>
        /// Triggered when the client is connected to the remote end point.
        /// </summary>
        protected abstract void OnConnected();

        /// <summary>
        /// Triggered when the client is disconnected from the remote end point.
        /// </summary>
        protected abstract void OnDisconnected();

        /// <summary>
        /// Triggered when a error on the socket happend
        /// </summary>
        /// <param name="socketError"></param>
        protected abstract void OnSocketError(SocketError socketError);

        /// <summary>
        /// Split an incoming buffer from the network in a collection of <see cref="NetPacketBase"/>.
        /// </summary>
        /// <param name="buffer">Incoming data</param>
        /// <returns></returns>
        protected virtual IReadOnlyCollection<NetPacketBase> SplitPackets(byte[] buffer) => NetPacket.Split(buffer);

        /// <summary>
        /// Sends the packets in the sending queue.
        /// </summary>
        private void ProcessSendingQueue()
        {
            while (true)
            {
                NetPacketBase packet = this._sendingQueue.Take();

                if (packet != null)
                {
                    byte[] buffer = packet.Buffer;

                    if (buffer.Length <= 0)
                        continue;

                    this._socketSendArgs.SetBuffer(buffer, 0, buffer.Length);
                    this._socket.SendAsync(this._socketSendArgs);
                    this._autoSendEvent.WaitOne();
                }
            }
        }

        /// <summary>
        /// Process and dispatch the received packets.
        /// </summary>
        private void ProcessReceiveQueue()
        {
            while (true)
            {
                var receivedMessage = this._receivingQueue.Take();

                if (receivedMessage != null)
                {
                    //this.HandleMessage(receivedMessage);
                    //receivedMessage.Dispose();
                }
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success && e.BytesTransferred > 0)
            {
                Console.WriteLine("Processing receive");

                var token = e.UserToken as AsyncUserToken;

                ProcessReceivedData(token.DataStartOffset, token.NextReceiveOffset - token.DataStartOffset + e.BytesTransferred, 0, token, e);

                token.NextReceiveOffset += e.BytesTransferred;

                if (token.NextReceiveOffset == e.Buffer.Length)
                {
                    token.NextReceiveOffset = 0;

                    if (token.DataStartOffset < e.Buffer.Length)
                    {
                        var notYesProcessDataSize = e.Buffer.Length - token.DataStartOffset;
                        Buffer.BlockCopy(e.Buffer, token.DataStartOffset, e.Buffer, 0, notYesProcessDataSize);

                        token.NextReceiveOffset = notYesProcessDataSize;
                    }

                    token.DataStartOffset = 0;
                }

                e.SetBuffer(token.NextReceiveOffset, e.Buffer.Length - token.NextReceiveOffset);

                if (!token.Socket.ReceiveAsync(e))
                    ProcessReceive(e);
            }
        }

        private void ProcessReceivedData(int dataStartOffset, int totalReceivedDataSize, int alreadyProcessedDataSize, AsyncUserToken token, SocketAsyncEventArgs e)
        {
            if (alreadyProcessedDataSize >= totalReceivedDataSize)
                return;

            if (token.MessageSize == null)
            {
                int headerSize = this.PacketProcessor.HeaderSize;

                if (totalReceivedDataSize > headerSize)
                {
                    var headerData = new byte[headerSize];
                    Buffer.BlockCopy(e.Buffer, dataStartOffset, headerData, 0, headerSize);
                    var messageSize = this.PacketProcessor.GetLength(headerData);

                    token.MessageSize = messageSize;
                    token.DataStartOffset = dataStartOffset + headerSize;

                    ProcessReceivedData(token.DataStartOffset, totalReceivedDataSize, alreadyProcessedDataSize + headerSize, token, e);
                }
            }
            else
            {
                // Read length
                var messageSize = token.MessageSize.Value;
                if (totalReceivedDataSize - alreadyProcessedDataSize >= messageSize)
                {
                    var messageData = new byte[messageSize];
                    Buffer.BlockCopy(e.Buffer, dataStartOffset, messageData, 0, messageSize);
                    this._receivingQueue.Add(messageData);

                    token.DataStartOffset = dataStartOffset + messageSize;
                    token.MessageSize = null;

                    ProcessReceivedData(token.DataStartOffset, totalReceivedDataSize, alreadyProcessedDataSize + messageSize, token, e);
                }
            }
        }

        /// <summary>
        /// Triggered when a <see cref="SocketAsyncEventArgs"/> async operation is completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (sender == null)
                throw new ArgumentNullException(nameof(sender));

            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    this._autoConnectEvent.Set();
                    this.OnConnected();
                    break;
                case SocketAsyncOperation.Receive:
                    this.ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    this._autoSendEvent.Set();
                    break;
                case SocketAsyncOperation.Disconnect:
                    this.OnDisconnected();
                    break;
                default: throw new InvalidOperationException("Unexpected socket async operation.");
            }
        }

        /// <summary>
        /// Creates a <see cref="SocketAsyncEventArgs"/>.
        /// </summary>
        /// <returns></returns>
        private static SocketAsyncEventArgs CreateSocketAsync(object userToken, int bufferSize, EventHandler<SocketAsyncEventArgs> completedAction)
        {
            var socketAsync = new SocketAsyncEventArgs()
            {
                UserToken = userToken
            };

            socketAsync.Completed += completedAction;

            if (bufferSize > 0)
                socketAsync.SetBuffer(new byte[bufferSize], 0, bufferSize);

            return socketAsync;
        }

        /// <summary>
        /// Dispose the <see cref="NetClient"/> instance.
        /// </summary>
        public virtual void Dispose()
        {
            this._autoConnectEvent.Dispose();
            this._autoSendEvent.Dispose();
            this.Disconnect();
        }
    }
}