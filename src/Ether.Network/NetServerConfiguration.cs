using System.Linq;
using System.Net;

namespace Ether.Network
{
    public sealed class NetServerConfiguration
    {
        public int Port { get; set; }

        public string Host { get; set; }

        public int Backlog { get; set; }

        public int MaximumNumberOfConnections { get; set; }

        public int ExcessConnectionAmount { get; set; }

        public int RecieveBufferSize { get; set; }

        internal int MaximumNumberOfRequests => this.MaximumNumberOfConnections + this.ExcessConnectionAmount;

        internal IPAddress Address
        {
            get
            {
                var host = Dns.GetHostAddressesAsync(this.Host).Result.First().ToString();

                return IPAddress.TryParse(host, out IPAddress address) ? address : null;
            }
        }

        public NetServerConfiguration()
        {
            this.Port = 0;
            this.Host = null;
            this.Backlog = 50;
            this.MaximumNumberOfConnections = 100;
            this.ExcessConnectionAmount = 1;
            this.RecieveBufferSize = 8192;
        }
    }
}
