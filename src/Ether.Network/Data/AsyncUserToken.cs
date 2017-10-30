using System;
using Ether.Network.Interfaces;

namespace Ether.Network.Data
{
    internal sealed class AsyncUserToken : IAsyncUserToken
    {
        public int? MessageSize { get; set; }

        public int DataStartOffset { get; set; }

        public int NextReceiveOffset { get; set; }

        public int TotalReceivedDataSize { get; set; }

        public Action<byte[]> MessageHandler { get; set; }

        public AsyncUserToken()
            : this(null)
        {
        }

        public AsyncUserToken(Action<byte[]> messageHandlerAction)
        {
            this.MessageHandler = messageHandlerAction;
        }
    }
}