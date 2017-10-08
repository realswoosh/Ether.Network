using System;

namespace Ether.Network
{
    /// <summary>
    /// Defines the packet data structrure.
    /// </summary>
    public struct PacketData : IEquatable<PacketData>
    {
        /// <summary>
        /// Gets the packet sender.
        /// </summary>
        public NetConnection Sender { get; private set; }

        /// <summary>
        /// Gets the packet data buffer.
        /// </summary>
        public byte[] Data { get; private set; }

        /// <summary>
        /// Creates a new <see cref="NetConnection"/> object.
        /// </summary>
        /// <param name="sender">Packet sender</param>
        /// <param name="data">Packet data</param>
        public PacketData(NetConnection sender, byte[] data)
        {
            this.Sender = sender;
            this.Data = data;
        }

        /// <summary>
        /// Compares two <see cref="PacketData"/>.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(PacketData other)
        {
            return this.Sender == other.Sender && this.Data == other.Data;
        }
    }
}
