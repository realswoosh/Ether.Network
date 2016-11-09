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
        }

        protected override void OnClientConnected(NetConnection client)
        {
            Console.WriteLine("New client connected. Id: {0}", client.Id);
        }

        protected override void OnClientDisconnected(NetConnection client)
        {
            Console.WriteLine("Client disconnected");
        }

        protected override void Initialize()
        {
            // TODO: initialize specific server resources at startup.
        }

        protected override void Idle()
        {
            Console.WriteLine("Server started! Listening on port {0}", this.Configuration.Port);
            // TODO: do custom process on main thread.
            while (true)
            {
                Console.ReadKey();
            }
        }
    }
}
