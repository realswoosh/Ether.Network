using System.Net;
using Ether.Network.Exceptions;
using Ether.Network.Utils;

namespace Ether.Network.Client
{
    /// <summary>
    /// Provides properties to configure a <see cref="NetClient"/>.
    /// </summary>
    public sealed class NetClientConfiguration
    {
        private readonly INetClient _client;
        private int _port;
        private int _bufferSize;
        private string _host;
        private int _timeOut;

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        public int Port
        {
            get => this._port;
            set => this.SetValue(ref this._port, value);
        }

        /// <summary>
        /// Gets or sets the buffer size.
        /// </summary>
        public int BufferSize
        {
            get => this._bufferSize;
            set => this.SetValue(ref this._bufferSize, value);
        }

        /// <summary>
        /// Gets or sets the connecting host.
        /// </summary>
        public string Host
        {
            get => this._host;
            set => this.SetValue(ref this._host, value);
        }

        /// <summary>
        /// Gets or sets the connecting time out.
        /// </summary>
        public int TimeOut
        {
            get => this._timeOut;
            set => this.SetValue(ref this._timeOut, value);
        }

        /// <summary>
        /// Gets the listening address.
        /// </summary>
        internal IPAddress Address => NetUtils.GetIpAddress(this._host);

        /// <summary>
        /// Creates a new <see cref="NetClientConfiguration"/> instance.
        /// </summary>
        /// <param name="client"></param>
        internal NetClientConfiguration(INetClient client)
        {
            this._client = client;
            this._bufferSize = 1024;
            this._port = 0;
            this._host = null;
            this._timeOut = 5000;
        }

        /// <summary>
        /// Set the value of a property passed as reference.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="container"></param>
        /// <param name="value"></param>
        private void SetValue<T>(ref T container, T value)
        {
            if (this._client != null && this._client.IsRunning)
                throw new EtherConfigurationException("Cannot change configuration once the client is running.");

            if (!Equals(container, value))
                container = value;
        }
    }
}
