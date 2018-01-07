namespace Ether.Network.Interfaces
{
    /// <summary>
    /// Defines the behavior of an Ether.Network connected user.
    /// </summary>
    public interface INetUser : INetConnection
    {
        /// <summary>
        /// Sends a packet throught the network.
        /// </summary>
        /// <param name="packet">Outgoing packet</param>
        void Send(INetPacketStream packet);

        /// <summary>
        /// Handles an incoming message.
        /// </summary>
        /// <param name="packet">Incoming packet</param>
        void HandleMessage(INetPacketStream packet);
    }
}
