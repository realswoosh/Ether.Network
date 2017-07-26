using Ether.Network.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Ether.Network
{
    public abstract class NetServer<T> : INetServer where T : NetConnection, new()
    {
        private bool _isRunning;
        private ManualResetEvent _resetEvent;
        private SocketAsyncEventArgs _acceptArgs;
        private BufferManager _bufferManager;
        private Semaphore _semaphore;

        protected Socket Socket { get; private set; }

        protected NetServerConfiguration Configuration { get; private set; }

        public bool IsRunning => this._isRunning;

        protected NetServer()
        {
            this.Configuration = new NetServerConfiguration(this);
            this.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this._resetEvent = new ManualResetEvent(false);
            this._isRunning = false;
            this._acceptArgs = new SocketAsyncEventArgs();
            this._acceptArgs.Completed += this.AcceptCompleted;
        }

        public void Start()
        {
            if (this._isRunning)
                throw new InvalidOperationException("Server is already running.");
            
            if (this.Configuration.Port <= 0)
                throw new EtherConfigurationException($"{this.Configuration.Port} is not a valid port."); // Invalid port

            var address = this.Configuration.Address;
            if (address == null)
                throw new EtherConfigurationException($"Invalid host : {this.Configuration.Host}"); // Invalid ip or host

            int maxNumberOfConnections = this.Configuration.MaximumNumberOfConnections;

            this._semaphore = new Semaphore(maxNumberOfConnections, maxNumberOfConnections);
            this._bufferManager = new BufferManager(maxNumberOfConnections, this.Configuration.BufferSize);
            this.Initialize();

            this.Socket.Bind(new IPEndPoint(address, this.Configuration.Port));
            this.Socket.Listen(this.Configuration.Backlog);
            this.StartAccept();

            this._isRunning = true;
            this._resetEvent.WaitOne();
        }

        public void Stop()
        {
            this._isRunning = false;
            this._resetEvent.Set();
        }

        protected abstract void Initialize();

        protected abstract void OnClientConnected();

        protected abstract void OnClientDisconnected();

        private void StartAccept()
        {
            if (this._acceptArgs.AcceptSocket != null)
                this._acceptArgs.AcceptSocket = null;
            if (!this.Socket.AcceptAsync(this._acceptArgs))
                this.ProcessAccept(this._acceptArgs);
        }

        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                NetConnection client = new T();
                client.Initialize(e.AcceptSocket, e, this.Configuration.BufferSize);
                
                this.OnClientConnected();
            }

            this.StartAccept();
        }

        private void AcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            this.ProcessAccept(e);
        }
    }
}
