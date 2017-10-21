namespace Ether.Network.Core
{
    public interface INetPacketStream
    {
        int Size { get; }

        long Position { get; }

        byte[] Buffer { get; }

        T Read<T>();

        void Write<T>(T value);
    }
}
