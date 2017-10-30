using Ether.Network.Interfaces;
using System;

namespace Ether.Network.Data
{
    internal struct MessageData : IEquatable<MessageData>
    {
        public INetUser User { get; }

        public byte[] Message { get; }

        public MessageData(INetUser user, byte[] message)
        {
            this.User = user;
            this.Message = message;
        }

        public bool Equals(MessageData other)
        {
            return this.User.Id == other.User.Id
                && this.Message == other.Message;
        }
    }
}
