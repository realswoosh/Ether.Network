using Ether.Network.Data;
using Ether.Network.Interfaces;
using System;

namespace Ether.Network
{
    /// <inheritdoc />
    public abstract class NetUser : NetConnection, INetUser
    {
        /// <summary>
        /// Gets the user token.
        /// </summary>
        internal IAsyncUserToken Token { get; }

        /// <summary>
        /// Gets or sets the send packet action.
        /// </summary>
        internal Action<INetUser, byte[]> SendAction { private get; set; }

        /// <summary>
        /// Creates a new <see cref="NetUser"/> instance.
        /// </summary>
        protected NetUser()
        {
            this.Token = new AsyncUserToken();
        }

        /// <inheritdoc />
        public virtual void HandleMessage(INetPacketStream packet)
        {
        }

        /// <inheritdoc />
        public virtual void Send(INetPacketStream packet)
        {
            this.SendAction?.Invoke(this, packet.Buffer);
        }
    }
}
