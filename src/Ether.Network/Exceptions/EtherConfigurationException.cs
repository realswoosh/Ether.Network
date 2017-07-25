namespace Ether.Network.Exceptions
{
    public class EtherConfigurationException : EtherException
    {
        public EtherConfigurationException()
            : base("")
        {

        }

        public EtherConfigurationException(string message) 
            : base(message)
        {
        }
    }
}
