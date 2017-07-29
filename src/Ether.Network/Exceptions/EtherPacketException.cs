using System;

namespace Ether.Network.Exceptions
{
    public class EtherPacketException : EtherException
    {
        public EtherPacketException(string message)
            : base(message)
        {
        }

        public EtherPacketException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
