using System;

namespace Ether.Network
{
    public abstract partial class NetServer<T> where T : NetConnection, new()
    {
        private struct PacketData : IEquatable<PacketData>
        {
            public NetConnection Sender { get; private set; }

            public byte[] Data { get; private set; }

            public PacketData(NetConnection sender, byte[] data)
            {
                this.Sender = sender;
                this.Data = data;
            }

            public bool Equals(PacketData other)
            {
                return this.Sender == other.Sender && this.Data == other.Data;
            }
        }

        #endregion
    }
}
