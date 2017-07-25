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
    public abstract class NetServer<T> where T : NetConnection, new()
    {
        private static readonly int recieveSendOpAloc = 2;

        private SocketAsyncEventArgsPool _acceptPool;
        private SocketAsyncEventArgsPool _handlerPool;
        private BufferManager _bufferManager;
        private Semaphore _semaphore;

        protected Socket Socket { get; private set; }

        protected NetServerConfiguration Configuration { get; private set; }

        protected NetServer()
        {
            this.Configuration = new NetServerConfiguration();
            this.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start()
        {
            if (this.Configuration.Port <= 0)
                throw new EtherConfigurationException($"{this.Configuration.Port} is not a valid port."); // Invalid port

            var address = this.Configuration.Address;
            if (address == null)
                throw new EtherConfigurationException(); // Invalid ip or host

            int maxNumberOfConnections = this.Configuration.MaximumNumberOfConnections;

            this._semaphore = new Semaphore(maxNumberOfConnections, maxNumberOfConnections);
            this._bufferManager = new BufferManager(
                this.Configuration.RecieveBufferSize * this.Configuration.MaximumNumberOfConnections * recieveSendOpAloc,
                this.Configuration.RecieveBufferSize);
            this._bufferManager.Initialize();
            
            this._acceptPool = new SocketAsyncEventArgsPool(maxNumberOfConnections);
            for (int i = 0; i < maxNumberOfConnections; i++)
                this._acceptPool.Push(this.CreateForAccept());

            this._handlerPool = new SocketAsyncEventArgsPool(maxNumberOfConnections);
            for (int i = 0; i < maxNumberOfConnections; i++)
            {
                SocketAsyncEventArgs socketAsyncEventArgs = this.CreateForReceive();
                this._bufferManager.SetBuffer(socketAsyncEventArgs);
                this._handlerPool.Push(socketAsyncEventArgs);
            }

            this.Initialize();

            this.Socket.Bind(new IPEndPoint(address, this.Configuration.Port));
            this.Socket.Listen(this.Configuration.Backlog);
            this.StartAccept();
        }

        public void Stop()
        {
            throw new NotSupportedException();
        }

        private void StartAccept()
        {
            SocketAsyncEventArgs acceptEventArg;

            if (this._acceptPool.Count > 1)
            {
                try
                {
                    acceptEventArg = this._acceptPool.Pop();
                }
                catch
                {
                    acceptEventArg = this.CreateForAccept();
                }
            }
            else
                acceptEventArg = this.CreateForAccept();

            this._semaphore.WaitOne();
            bool raiseEvent = this.Socket.AcceptAsync(acceptEventArg);
            
            if (!raiseEvent)
                ProcessAccept(acceptEventArg);
        }

        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                this.StartAccept();

                (e.UserToken as T).Socket.Shutdown(SocketShutdown.Both);
                this._acceptPool.Push(e);
            }

            this.StartAccept();

            var receiveSendEventArgs = this._handlerPool.Pop();
            receiveSendEventArgs.AcceptSocket = e.AcceptSocket;
            e.AcceptSocket = null;
            this._acceptPool.Push(e);

            this.StartReceive(receiveSendEventArgs);
        }

        private void StartReceive(SocketAsyncEventArgs e)
        {
        }

        private SocketAsyncEventArgs CreateForAccept()
        {
            var saea = new SocketAsyncEventArgs();

            saea.Completed += this.Accept_Completed;
            saea.UserToken = new NetConnection();

            return saea;
        }

        private SocketAsyncEventArgs CreateForReceive()
        {
            var saea = new SocketAsyncEventArgs();

            saea.Completed += this.IO_Completed;
            saea.UserToken = new T();

            return saea;
        }

        private void Accept_Completed(object sender, SocketAsyncEventArgs e)
        {
            this.ProcessAccept(e);
        }

        private void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
        }

        protected abstract void Initialize();
    }
}
