namespace Ether.Network.Interfaces
{
    public interface INetUser : INetConnection
    {
        void Send();

        void HandleMessage();
    }
}
