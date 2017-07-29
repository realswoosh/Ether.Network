using System.Net;
using System.Linq;

namespace Ether.Network.Utils
{
    internal static class NetUtils
    {
        public static IPAddress GetIpAddress(string ipOrHost)
        {
            var host = Dns.GetHostAddressesAsync(ipOrHost).Result.First().ToString();

            return IPAddress.TryParse(host, out IPAddress address) ? address : null;
        }
    }
}
