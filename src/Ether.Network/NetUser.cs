using Ether.Network.Interfaces;
using System;

namespace Ether.Network
{
    public abstract class NetUser : NetConnection, INetUser
    {
        internal IAsyncUserToken Token { get; }

        protected NetUser()
        {
            this.Token = new AsyncUserToken();
        }

        public virtual void HandleMessage(INetPacketStream packet)
        {
            throw new NotImplementedException();
        }

        public virtual void Send(INetPacketStream packet)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
