using Ether.Network.Data;
using Ether.Network.Interfaces;
using System;

namespace Ether.Network
{
    public abstract class NetUser : NetConnection, INetUser
    {
        internal IAsyncUserToken Token { get; }

        internal Action<INetUser, byte[]> SendAction { get; set; }

        protected NetUser()
        {
            this.Token = new AsyncUserToken();
        }

        public virtual void HandleMessage(INetPacketStream packet)
        {
        }

        public virtual void Send(INetPacketStream packet)
        {
            this.SendAction?.Invoke(this, packet.Buffer);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
