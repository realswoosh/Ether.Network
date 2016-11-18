using Ether.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Ether.Network
{
    /// <summary>
    /// NetServer 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class NetServer<T> : IDisposable where T : NetConnection, new()
    {
        private static object syncClients = new object();
        
        private Socket listenSocket;
        private Thread listenThread;
        private Thread handlerThread;
        private List<NetConnection> clients;
        
        /// <summary>
        /// Gets the NetServer clients list.
        /// </summary>
        public IReadOnlyCollection<NetConnection> Clients
        {
            get { return this.clients; }
        }

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
            : base()
        {
            this.IsRunning = false;
            this.clients = new List<NetConnection>();

            this.Configuration = new NetConfiguration()
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
                this.IsRunning = true;

                if (configuration != null)
                    this.Configuration = configuration;

                this.Initialize();

                this.listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.listenSocket.Bind(new IPEndPoint(this.Configuration.IpAddress, this.Configuration.Port));
                this.listenSocket.Listen(100);

                this.listenThread = new Thread(this.ListenSocket);
                this.listenThread.Start();

                this.handlerThread = new Thread(this.HandleClients);
                this.handlerThread.Start();

                NetDelayer.Start();

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
            if (!this.IsRunning)
                return;

            this.IsRunning = false;
            NetDelayer.Stop();
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

                    this.OnClientConnected(client);
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
                            if (client.Socket.Poll(10, SelectMode.SelectRead))
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

                            if (recievedDataSize < 1)
                            {
                                this.RemoveClient(client);
                                continue;
                            }

                            var recievedPackets = this.SplitPackets(buffer);

                            foreach (var packet in recievedPackets)
                            {
                                client.HandleMessage(packet);
                                packet.Dispose();
                            }
                        }
                        catch (Exception e)
                        {
#if DEBUG
                            Console.WriteLine($"Error: {Environment.NewLine}{e.Message}{Environment.NewLine}{e.StackTrace}");
#endif
                            if (client.Socket.Connected == false)
                                this.RemoveClient(client);

                            continue;
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
        /// <param name="client">Client to remove</param>
        public void RemoveClient(NetConnection client)
        {
            lock (syncClients)
            {
                var clientToRemove = this.clients.Find(item => item != null && item.Id == client.Id);

                if (clientToRemove == null)
                    Console.WriteLine("Cannot remove client. Unknow client {0}", client.Id);

                this.clients.Remove(clientToRemove);
                clientToRemove.Dispose();
                this.OnClientDisconnected(clientToRemove);
            }
        }

        /// <summary>
        /// Split a buffer into packets.
        /// </summary>
        /// <param name="buffer">Incoming buffer</param>
        /// <returns></returns>
        protected virtual IReadOnlyCollection<NetPacketBase> SplitPackets(byte[] buffer)
        {
            return NetPacket.Split(buffer);
        }

        /// <summary>
        /// Initialize server internal resources.
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        /// Waits for user input.
        /// </summary>
        protected abstract void Idle();

        /// <summary>
        /// On client connected.
        /// </summary>
        /// <param name="client">Connected client</param>
        protected abstract void OnClientConnected(NetConnection client);

        /// <summary>
        /// On client disconnected.
        /// </summary>
        /// <param name="client">Disconnected client</param>
        protected abstract void OnClientDisconnected(NetConnection client);

        #region IDisposable Support
        private bool disposedValue;

        /// <summary>
        /// Dispose the resources.
        /// </summary>
        /// <param name="disposing">Dispose or not the managed resources</param>
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

                foreach (var client in this.clients)
                    client.Dispose();

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
