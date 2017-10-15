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

namespace Ether.Network
{
    /// <summary>
    /// Managed TCP socket server.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class NetServer<T> : INetServer, IDisposable where T : NetConnection, new()
    {
        private static readonly string AllInterfaces = "0.0.0.0";

        private readonly ConcurrentDictionary<Guid, T> _clients;
        private readonly ConcurrentQueue<PacketData> _messageQueue;
        private readonly ManualResetEvent _resetEvent;
        private readonly AutoResetEvent _sendEvent;
        private readonly AutoResetEvent _sendQueueNotifier;
        private readonly SocketAsyncEventArgs _acceptArgs;

        private bool _isRunning;
        private SocketAsyncEventArgsPool _readPool;
        private SocketAsyncEventArgsPool _writePool;

        /// <summary>
        /// Gets the <see cref="NetServer{T}"/> listening socket.
        /// </summary>
        protected Socket Socket { get; private set; }

        /// <summary>
        /// Gets the <see cref="NetServer{T}"/> configuration
        /// </summary>
        protected NetServerConfiguration Configuration { get; private set; }

        /// <summary>
        /// Gets the <see cref="NetServer{T}"/> running state.
        /// </summary>
        public bool IsRunning => this._isRunning;

        /// <summary>
        /// Gets the connected client.
        /// </summary>
        public IReadOnlyCollection<T> Clients => this._clients.Values as IReadOnlyCollection<T>;

        /// <summary>
        /// Creates a new <see cref="NetServer{T}"/> instance.
        /// </summary>
        protected NetServer()
        {
            this.Configuration = new NetServerConfiguration(this);
            this.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this._clients = new ConcurrentDictionary<Guid, T>();
            this._messageQueue = new ConcurrentQueue<PacketData>();
            this._resetEvent = new ManualResetEvent(false);
            this._sendEvent = new AutoResetEvent(false);
            this._sendQueueNotifier = new AutoResetEvent(false);
            this._isRunning = false;
            this._acceptArgs = new SocketAsyncEventArgs();
            this._acceptArgs.Completed += this.IO_Completed;
        }

        /// <summary>
        /// Initialize and start the server.
        /// </summary>
        public void Start()
        {
            if (this._isRunning)
                throw new InvalidOperationException("Server is already running.");

            if (this.Configuration.Port <= 0)
                throw new EtherConfigurationException($"{this.Configuration.Port} is not a valid port.");

            var address = this.Configuration.Host == AllInterfaces ? IPAddress.Any : this.Configuration.Address;
            if (address == null)
                throw new EtherConfigurationException($"Invalid host : {this.Configuration.Host}");

            if (this._readPool == null)
            {
                this._readPool = new SocketAsyncEventArgsPool();

                for (int i = 0; i < this.Configuration.MaximumNumberOfConnections; i++)
                    this._readPool.Push(NetUtils.CreateSocket(this.Configuration.BufferSize, this.IO_Completed));
            }

            if (this._writePool == null)
            {
                this._writePool = new SocketAsyncEventArgsPool();

                for (int i = 0; i < this.Configuration.MaximumNumberOfConnections; i++)
                    this._writePool.Push(NetUtils.CreateSocket(this.Configuration.BufferSize, this.IO_Completed));
            }

            this.Initialize();
            this.Socket.Bind(new IPEndPoint(address, this.Configuration.Port));
            this.Socket.Listen(this.Configuration.Backlog);

            Task.Factory.StartNew(this.ProcessSendQueue);

            this.StartAccept();

            this._isRunning = true;
            this._resetEvent.WaitOne();
        }

        /// <summary>
        /// Stop the server.
        /// </summary>
        public void Stop()
        {
            if (this._isRunning)
            {
                this._isRunning = false;
                this._resetEvent.Set();
            }
        }

        /// <summary>
        /// Disconnects the client from this server.
        /// </summary>
        /// <param name="clientId">Client unique Id</param>
        public void DisconnectClient(Guid clientId)
        {
            if (!this._clients.ContainsKey(clientId))
                throw new EtherClientNotFoundException(clientId);

            if (this._clients.TryRemove(clientId, out T removedClient))
            {
                removedClient.Dispose();
                this.OnClientDisconnected(removedClient);
            }
        }

        /// <summary>
        /// Initialize the server resourrces.
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        /// Triggered when a new client is connected to the server.
        /// </summary>
        /// <param name="connection"></param>
        protected abstract void OnClientConnected(T connection);

        /// <summary>
        /// Triggered when a client disconnects from the server.
        /// </summary>
        /// <param name="connection"></param>
        protected abstract void OnClientDisconnected(T connection);

        /// <summary>
        /// Triggered when an error occurs on the server.
        /// </summary>
        /// <param name="exception">Exception</param>
        protected abstract void OnError(Exception exception);

        /// <summary>
        /// Split an incoming network buffer.
        /// </summary>
        /// <param name="buffer">Incoming data buffer</param>
        /// <returns>Readonly collection of <see cref="NetPacketBase"/></returns>
        protected virtual IReadOnlyCollection<NetPacketBase> SplitPackets(byte[] buffer)
        {
            return NetPacket.Split(buffer);
        }

        /// <summary>
        /// Starts the accept connection async operation.
        /// </summary>
        private void StartAccept()
        {
            if (this._acceptArgs.AcceptSocket != null)
                this._acceptArgs.AcceptSocket = null;
            if (!this.Socket.AcceptAsync(this._acceptArgs))
                this.ProcessAccept(this._acceptArgs);
        }

        /// <summary>
        /// Process the accept connection async operation.
        /// </summary>
        /// <param name="e"></param>
        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                var client = new T();
                client.Initialize(e.AcceptSocket, this.SendData);

                if (!this._clients.TryAdd(client.Id, client))
                    throw new EtherException($"Client {client.Id} already exists in client list.");

                this.OnClientConnected(client);

                SocketAsyncEventArgs readArgs = this._readPool.Pop();
                readArgs.UserToken = client;

                if (e.AcceptSocket != null && !e.AcceptSocket.ReceiveAsync(readArgs))
                    this.ProcessReceive(readArgs);
            }

            e.AcceptSocket = null;
            this.StartAccept();
        }

        /// <summary>
        /// Process the receive async operation on one <see cref="SocketAsyncEventArgs"/>.
        /// </summary>
        /// <param name="e"></param>
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            var netConnection = e.UserToken as T;

            if (e.SocketError == SocketError.Success && e.BytesTransferred > 0)
            {
                this.DispatchPackets(netConnection, e);
                if (netConnection.Socket != null && !netConnection.Socket.ReceiveAsync(e))
                    this.ProcessReceive(e);
            }
            else
            {
                this.DisconnectClient(netConnection.Id);
                e.UserToken = null;
                this._readPool.Push(e);
            }
        }

        /// <summary>
        /// Split and dispatch incoming packets to the <see cref="NetConnection"/>.
        /// </summary>
        /// <param name="netConnection"></param>
        /// <param name="e"></param>
        private void DispatchPackets(NetConnection netConnection, SocketAsyncEventArgs e)
        {
            byte[] buffer = NetUtils.GetPacketBuffer(e.Buffer, e.Offset, e.BytesTransferred);
            IReadOnlyCollection<NetPacketBase> packets = this.SplitPackets(buffer);

            foreach (var packet in packets)
            {
                try
                {
                    netConnection.HandleMessage(packet);
                }
                catch (Exception exception)
                {
                    this.OnError(exception);
                }
            }
        }

        /// <summary>
        /// Send data to a <see cref="NetConnection"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="buffer"></param>
        private void SendData(NetConnection sender, byte[] buffer)
        {
            var newPacket = new PacketData(sender, buffer);
            this._messageQueue.Enqueue(newPacket);
            this._sendQueueNotifier.Set();
        }

        /// <summary>
        /// Process the send queue.
        /// </summary>
        private void ProcessSendQueue()
        {
            while (true)
            {
                this._sendQueueNotifier.WaitOne();

                if (this._messageQueue.TryDequeue(out PacketData packet))
                {
                    this.Send(packet);
                }
            }
        }

        /// <summary>
        /// Send the packet through the network.
        /// </summary>
        /// <param name="packet"></param>
        private void Send(PacketData packet)
        {
            SocketAsyncEventArgs socketEvent = this._writePool.Pop();

            if (socketEvent != null)
            {
                socketEvent.SetBuffer(packet.Data, 0, packet.Data.Length);
                socketEvent.UserToken = packet.Sender;
                packet.Sender.Socket.SendAsync(socketEvent);
            }
            else
            {
                this._sendEvent.WaitOne();
                this.Send(packet);
            }
        }

        /// <summary>
        /// Process the send async operation.
        /// </summary>
        /// <param name="e"></param>
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            this._writePool.Push(e);
            this._sendEvent.Set();
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
                case SocketAsyncOperation.Accept:
                    this.ProcessAccept(e);
                    break;
                case SocketAsyncOperation.Receive:
                    this.ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    this.ProcessSend(e);
                    break;
                case SocketAsyncOperation.Disconnect: break;
                default:
                    throw new InvalidOperationException("Unexpected SocketAsyncOperation.");
            }
        }

        #region IDisposable Support

        private bool _disposed;

        /// <summary>
        /// Dispose the <see cref="NetServer{T}"/> resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    this.Stop();

                    if (this.Socket != null)
                    {
                        this.Socket.Shutdown(SocketShutdown.Both);
                        this.Socket.Dispose();
                        this.Socket = null;
                    }

                    if (this._readPool != null)
                    {
                        this._readPool.Dispose();
                        this._readPool = null;
                    }

                    if (this._writePool != null)
                    {
                        this._writePool.Dispose();
                        this._writePool = null;
                    }
                }

                _disposed = true;
            }
            else
                throw new ObjectDisposedException(nameof(NetServer<T>));
        }

        /// <summary>
        /// Destroys the <see cref="NetServer{T}"/> instance.
        /// </summary>
        ~NetServer()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Dispose the <see cref="NetServer{T}"/> resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
