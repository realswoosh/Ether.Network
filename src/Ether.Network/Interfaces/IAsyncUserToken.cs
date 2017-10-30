using System;

namespace Ether.Network.Interfaces
{
    internal interface IAsyncUserToken
    {
        int? MessageSize { get; set; }

        int DataStartOffset { get; set; }

        int NextReceiveOffset { get; set; }

        int TotalReceivedDataSize { get; set; }

        Action<byte[]> MessageHandler { get; set; }
    }
}
