using System;

namespace Ether.Network.Exceptions
{
    public class EtherException : Exception
    {
        public EtherException(string message)
            : this(message, null)
        {
        }

        public EtherException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
