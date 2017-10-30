namespace Ether.Network.Interfaces
{
    public interface INetUser : INetConnection
    {
        void Send(INetPacketStream packet);

        void HandleMessage(INetPacketStream packet);
    }
}
