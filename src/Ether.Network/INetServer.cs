namespace Ether.Network
{
    public interface INetServer
    {
        bool IsRunning { get; }
        void Start();
        void Stop();
    }
}
