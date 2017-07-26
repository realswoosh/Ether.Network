using Ether.Network.Exceptions;
using System.Linq;
using System.Net;

namespace Ether.Network
{
    public sealed class NetServerConfiguration
    {
        private readonly INetServer _server;
        private int _port;
        private int _backlog;
        private int _bufferSize;
        private int _maximumNumberOfConnections;
        private string _host;

        public int Port
        {
            get { return this._port; }
            set { this.SetValue(ref this._port, value); }
        }

        public string Host
        {
            get { return this._host; }
            set { this.SetValue(ref this._host, value); }
        }

        public int Backlog
        {
            get { return this._backlog; }
            set { this.SetValue(ref this._backlog, value); }
        }

        public int MaximumNumberOfConnections
        {
            get { return this._maximumNumberOfConnections; }
            set { this.SetValue(ref this._maximumNumberOfConnections, value); }
        }

        public int BufferSize
        {
            get { return this._bufferSize; }
            set { this.SetValue(ref this._bufferSize, value); }
        }

        internal IPAddress Address
        {
            get
            {
                var host = Dns.GetHostAddressesAsync(this.Host).Result.First().ToString();

                return IPAddress.TryParse(host, out IPAddress address) ? address : null;
            }
        }
        
        public NetServerConfiguration()
            : this(null)
        {
        }

        internal NetServerConfiguration(INetServer server)
        {
            this._server = server;
            this.Port = 0;
            this.Host = null;
            this.Backlog = 50;
            this.MaximumNumberOfConnections = 100;
            this.BufferSize = 4096;
        }

        private void SetValue<T>(ref T container, T value)
        {
            if (this._server != null && this._server.IsRunning && !container.Equals(value))
                throw new EtherConfigurationException("Cannot change configuration once the server is running.");

            container = value;
        }
    }
}
