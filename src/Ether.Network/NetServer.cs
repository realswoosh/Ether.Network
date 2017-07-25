using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Ether.Network
{
    public abstract class NetServer<T> where T : NetConnection, new()
    {
        protected Socket socket;
        private SocketAsyncEventArgsPool _acceptPool;
        private SocketAsyncEventArgsPool _handlerPool;
        private BufferManager _bufferManager;

        protected NetServer()
        {
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start()
        {
            throw new NotSupportedException();
        }

        public void Stop()
        {
            throw new NotSupportedException();
        }
    }
}
