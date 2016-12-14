namespace Ether.Network.Enums
{
    /// <summary>
    /// Enum representing the different states of a packet.
    /// </summary>
    internal enum PacketStateType
    {
        /// <summary>
        /// Read-only packet.
        /// </summary>
        Read = 0,

        /// <summary>
        /// Write-only packet.
        /// </summary>
        Write,
    }
}
