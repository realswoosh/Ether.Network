# Ether.Network 

[![Build Status](https://travis-ci.org/Eastrall/Ether.Network.svg?branch=master)](https://travis-ci.org/Eastrall/Ether.Network)
[![NuGet Status](https://img.shields.io/nuget/v/Ether.Network.svg)](https://www.nuget.org/packages/Ether.Network/)

Ether.Network is a basic library to make quickly a simple server or client using sockets.

This library is coded in C# using .NET Core framework to target Windows and Linux operating systems.

For now we use the basic synchronous sockets, and in the future we'll add the support of asynchronous sockets to increase performances and stability.

## How to install

Create a .NETCore project and add the nuget package: `Ether.Network` or you can do it manually :

`Install-Package Ether.Network` in your package manager console.

## How to use

There is a sample server application:

```c#
using Ether.Network;
using Ether.Network.Packets;
using System;
using System.Net.Sockets;

namespace MyServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var server = new Server())
                server.Start();
        }
    }

    public class Server : NetServer<Client>
    {
        public Server()
            : base()
        {
            this.Configuration = new NetConfiguration()
            {
                Ip = "127.0.0.1",
                Port = 4444
            };
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

        protected override void Initialize()
        {
            // Load resources
        }
    }

    public class Client : NetConnection
    {
        public Client()
            : base()
        {
        }

        public Client(Socket socket)
            : base(socket)
        {
        }

        public override void Greetings()
        {
            // say hi to the connected client
            var hiPacket = new NetPacket();

            hiPacket.Write(42); // packet header
            hiPacket.Write("Hello client!");

            this.Send(hiPacket);
        }

        public override void HandleMessage(NetPacket packet)
        {
            int value = packet.Read<int>(); // packet header
            string message = packet.Read<string>();

            Console.WriteLine("message from client: {0}", message);

            base.HandleMessage(packet);
        }
    }
}
```

And now a simple client app

```c#
using Ether.Network;
using Ether.Network.Packets;
using System;

namespace MyClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "Client";

            using (var client = new Client())
            {
                client.Connect("127.0.0.1", 4444);
                client.Run();
            }
        }
    }

    public class Client : NetClient
    {
        public Client()
            : base() { }

        public override void HandleMessage(NetPacket packet)
        {
            int header = packet.Read<int>();
            string message = packet.Read<string>();

            Console.WriteLine("Message from server: {0}", message);

            base.HandleMessage(packet);
        }
    }
}
```