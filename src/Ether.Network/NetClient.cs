using System;
using System.Collections.Generic;
using System.Text;
using Ether.Network.Packets;
using System.Net.Sockets;
using System.Net;
using Ether.Network.Utils;
using Ether.Network.Exceptions;

namespace Ether.Network
{
    public abstract class NetClient : NetConnection
    {
        private readonly string _host;
        private readonly int _port;
        private readonly int _bufferSize;
        private readonly SocketAsyncEventArgs _socketConnectArgs;
        private readonly SocketAsyncEventArgs _socketReceiveArgs;
        private readonly SocketAsyncEventArgs _socketSendArgs;

        private bool _isRunning;

        public bool IsRunning => this._isRunning;

        public NetClient(string host, int port, int bufferSize)
        {
            this._host = host;
            this._port = port;
            this._bufferSize = bufferSize;
            this._socketConnectArgs = this.CreateSocketAsync();
            this._socketSendArgs = this.CreateSocketAsync();
            this._socketReceiveArgs = this.CreateSocketAsync();
            this._socketReceiveArgs.SetBuffer(new byte[bufferSize], 0, bufferSize);
            this.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect()
        {
            if (this._isRunning)
                throw new InvalidOperationException("Client is already running");

            IPAddress address = NetUtils.GetIpAddress(this._host);

            if (address == null)
                throw new EtherConfigurationException($"Invalid host or ip address: {this._host}.");

            this.StartConnect(address, this._port);
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public void Send(NetPacketBase packet)
        {
            throw new NotImplementedException();
        }

        protected abstract void OnConnected();

        protected abstract void OnDisconnected();

        private void StartConnect(IPAddress address, int port)
        {
            this._socketConnectArgs.RemoteEndPoint = new IPEndPoint(address, this._port);

            if (!this.Socket.ConnectAsync(this._socketConnectArgs))
                this.ProcessConnect(this._socketConnectArgs);
        }

        private void ProcessConnect(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                this.OnConnected();
                this.StartReceive(this._socketReceiveArgs);
            }
        }

        private void StartReceive(SocketAsyncEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void StartSend(SocketAsyncEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ProcessSend(SocketAsyncEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (sender == null)
                throw new ArgumentNullException(nameof(sender));

            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    this.ProcessConnect(e);
                    break;
                case SocketAsyncOperation.Receive:
                    this.ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    this.ProcessSend(e);
                    break;
            }
        }

        private SocketAsyncEventArgs CreateSocketAsync()
        {
            var socketAsync = new SocketAsyncEventArgs()
            {
                UserToken = this.Socket
            };
            socketAsync.Completed += this.IO_Completed;

            return socketAsync;
        }

        public override void HandleMessage(NetPacketBase packet)
        {
        }
    }
}
