using System.Net;
using System.Linq;
using System;
using System.Net.Sockets;
using Ether.Network.Exceptions;

namespace Ether.Network.Utils
{
    internal static class NetUtils
    {
        public static IPAddress GetIpAddress(string ipOrHost)
        {
            var host = Dns.GetHostAddressesAsync(ipOrHost).Result.First().ToString();

            return IPAddress.TryParse(host, out IPAddress address) ? address : null;
        }

        public static IPEndPoint CreateIpEndPoint(string ipOrHost, int port)
        {
            IPAddress address = GetIpAddress(ipOrHost);

            if (address == null)
                throw new EtherConfigurationException($"Invalid host or ip address: {ipOrHost}.");
            if (port <= 0)
                throw new EtherConfigurationException($"Invalid port: {port}");

            return new IPEndPoint(address, port);
        }

        public static byte[] GetPacketBuffer(byte[] bufferSource, int offset, int size)
        {
            var buffer = new byte[size];

            Buffer.BlockCopy(bufferSource, offset, buffer, 0, size);

            return buffer;
        }

        public static SocketAsyncEventArgs CreateSocket(int bufferSize, EventHandler<SocketAsyncEventArgs> completedAction)
        {
            var socket = new SocketAsyncEventArgs();

            socket.Completed += completedAction;
            socket.SetBuffer(new Byte[bufferSize], 0, bufferSize);

            return socket;
        }
    }
}
