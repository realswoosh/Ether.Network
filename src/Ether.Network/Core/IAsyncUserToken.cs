namespace Ether.Network.Core
{
    public interface IAsyncUserToken
    {
        int? MessageSize { get; set; }

        int DataStartOffset { get; set; }

        int NextReceiveOffset { get; set; }

        int TotalReceivedDataSize { get; set; }
    }
}
