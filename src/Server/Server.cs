using Ether.Network;
using System;

namespace Server
{
    public class Server : NetServer<Client>
    {
        public Server()
            : base()
        {
            // Create the configuration
            this.Configuration = new NetConfiguration()
            {
                Ip = "127.0.0.1",
                Port = 4444
            };

            this.OnClientConnected += Server_OnClientConnected;
            this.OnClientDisconnected += Server_OnClientDisconnected;
        }

        private void Server_OnClientDisconnected(Object sender, NetConnection e)
        {
            Console.WriteLine("Client with unique id {0} disconnected.", e.Id);
        }

        private void Server_OnClientConnected(Object sender, NetConnection e)
        {
            Console.WriteLine("New client connected with unique id: {0}", e.Id);
        }

        protected override void Initialize()
        {
            // TODO: initialize specific server resources at startup.
        }

        protected override void Idle()
        {
            Console.WriteLine("Server started!");
            // TODO: do custom process on main thread.
            while (true)
            {
                Console.ReadKey();
            }
        }
    }
}
