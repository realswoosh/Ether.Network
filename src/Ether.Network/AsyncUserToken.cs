using Ether.Network.Interfaces;

namespace Ether.Network
{
    internal sealed class AsyncUserToken : IAsyncUserToken
    {
        public int? MessageSize { get; set; }

        public int DataStartOffset { get; set; }

        public int NextReceiveOffset { get; set; }

        public int TotalReceivedDataSize { get; set; }
    }
}