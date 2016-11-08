using Ether.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Ether.Network
{
    public abstract class NetServer<T> : IDisposable where T : NetConnection, new()
    {
        public event EventHandler<NetConnection> OnClientConnected;
        public event EventHandler<NetConnection> OnClientDisconnected;

        private static object syncClients = new object();

        private Socket listenSocket;
        private readonly List<NetConnection> clients;
        private Thread listenThread;
        private Thread handlerThread;

        private readonly NetConfiguration defaultConfiguration;

        /// <summary>
        /// Gets the NetServer configuration.
        /// </summary>
        public NetConfiguration Configuration { get; protected set; }

        /// <summary>
        /// Gets the value if the server is running.
        /// </summary>
        public bool IsRunning { get; private set; }
    
        /// <summary>
        /// Creates a new NetServer instance.
        /// </summary>
        public NetServer()
        {
            this.IsRunning = false;
            this.clients = new List<NetConnection>();

            this.defaultConfiguration = new NetConfiguration()
            {
                Ip = "127.0.0.1",
                Port = 5000
            };
        }

        /// <summary>
        /// Destroy the server.
        /// </summary>
        ~NetServer()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Start the server.
        /// </summary>
        /// <param name="configuration">NetServer configuration</param>
        public void Start(NetConfiguration configuration = null)
        {
            if (this.IsRunning == false)
            {
                this.Configuration = configuration == null ? this.defaultConfiguration : configuration;

                this.Initialize();

                this.listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.listenSocket.Bind(new IPEndPoint(this.Configuration.IpAddress, this.Configuration.Port));
                this.listenSocket.Listen(100);

                this.listenThread = new Thread(this.ListenSocket);
                this.listenThread.Start();

                this.handlerThread = new Thread(this.HandleClients);
                this.handlerThread.Start();

                this.IsRunning = true;

                this.Idle();
            }
            else
                throw new InvalidOperationException("NetServer is already running.");
        }

        /// <summary>
        /// Stop the server.
        /// </summary>
        public void Stop()
        {
            if (!this.IsRunning) return;
            this.IsRunning = false;
            this.Dispose();
        }

        /// <summary>
        /// Listen new clients on the socket.
        /// </summary>
        private void ListenSocket()
        {
            while (this.IsRunning)
            {
                if (this.listenSocket.Poll(100, SelectMode.SelectRead))
                {
                    var client = Activator.CreateInstance(typeof(T), this.listenSocket.Accept()) as T;
                    
                    lock (syncClients)
                        this.clients.Add(client);

                    this.OnClientConnected?.Invoke(this, client);
                }

                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Handle connected clients.
        /// </summary>
        private void HandleClients()
        {
            var clientsReady = new Queue<NetConnection>();

            try
            {
                while (this.IsRunning)
                {
                    lock (syncClients)
                    {
                        foreach (var client in this.clients)
                        {
                            if (client.Socket.Poll(100, SelectMode.SelectRead))
                                clientsReady.Enqueue(client);
                        }
                    }

                    while (clientsReady.Any())
                    {
                        var client = clientsReady.Dequeue();

                        try
                        {
                            var buffer = new byte[client.Socket.Available];
                            var recievedDataSize = client.Socket.Receive(buffer);

                            if (recievedDataSize <= 0)
                                throw new Exception("Disconnected from the server");
                            else
                            {
                                var recievedPackets = NetPacket.Split(buffer);

                                foreach (var packet in recievedPackets)
                                {
                                    client.HandleMessage(packet);
                                    packet.Dispose();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            if (!client.Socket.Connected)
                                this.RemoveClient(client);
                            else
                                Console.WriteLine($"Error: {e.Message}{Environment.NewLine}{e.StackTrace}");
                        }
                    }

                    Thread.Sleep(50);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected error: {e.Message}. StackTrace:{Environment.NewLine}{e.StackTrace}");
            }
        }

        /// <summary>
        /// Removes a client from the server.
        /// </summary>
        /// <param name="client"></param>
        public void RemoveClient(NetConnection client)
        {
            lock (syncClients)
            {
                var clientToRemove = this.clients.Find(item => item != null && item == client);

                this.clients.Remove(clientToRemove);
                clientToRemove.Dispose();
                this.OnClientDisconnected?.Invoke(this, clientToRemove);
            }
        }

        /// <summary>
        /// Initialize server internal resources.
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        /// Waits for user input.
        /// </summary>
        protected abstract void Idle();

        #region IDisposable Support
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue) return;
            if (disposing)
            {
                this.listenThread.Join();
                this.handlerThread.Join();

                this.listenThread = null;
                this.handlerThread = null;

                this.listenSocket.Dispose();

                foreach (var connection in this.clients)
                    connection.Dispose();

                this.clients.Clear();
            }

            disposedValue = true;
        }

        /// <summary>
        /// Dispose the server resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
