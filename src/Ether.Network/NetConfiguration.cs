using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Ether.Network
{
    public class NetConfiguration
    {
        /// <summary>
        /// Gets or sets the Ip address.
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// Gets the Ip adress.
        /// </summary>
        internal IPAddress IpAddress
        {
            get
            {
                IPAddress address;
                bool parseResult = IPAddress.TryParse(this.Ip, out address);

                return parseResult == true ? address : null;
            }
        }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        public int Port { get; set; }
    }
}
