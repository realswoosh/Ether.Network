namespace Ether.Network
{
    /// <summary>
    /// NetServer interface.
    /// </summary>
    public interface INetServer
    {
        /// <summary>
        /// Gets the <see cref="INetServer"/> running state.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Start the server.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop the server.
        /// </summary>
        void Stop();
    }
}
