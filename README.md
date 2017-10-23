# ![logo](https://raw.githubusercontent.com/Eastrall/Ether.Network/develop/banner.png)

[![forthebadge](http://forthebadge.com/images/badges/made-with-c-sharp.svg)](http://forthebadge.com)
[![forthebadge](http://forthebadge.com/images/badges/built-with-love.svg)](http://forthebadge.com)

[![Build Status](https://travis-ci.org/Eastrall/Ether.Network.svg?branch=develop)](https://travis-ci.org/Eastrall/Ether.Network)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/e84d77087d6940f79061799383cc1432)](https://www.codacy.com/app/Eastrall/Ether.Network?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=Eastrall/Ether.Network&amp;utm_campaign=Badge_Grade)
[![NuGet Status](https://img.shields.io/nuget/v/Ether.Network.svg)](https://www.nuget.org/packages/Ether.Network/)

Ether.Network is a basic library to make quickly a simple server or client using sockets.

This library is coded with C# using .NET Core framework to target Windows and Linux operating systems.

## Framework support

- .NET Core 1.0 (netstandard1.3)
- .NET Core 2.0 (netstandard2.0)
- .NET Framework 4.5

## How to install

Create a .NETCore project and add the nuget package: `Ether.Network` or you can do it manually :

`Install-Package Ether.Network` in your package manager console.

## How to use

### Server application

```c#
using Ether.Network;
using Ether.Network.Packets;
using System;

namespace ServerApp
{
	class Program
	{
		using (var server = new MyServer())
			server.Start();
	}

	class MyServer : NetServer<ClientConnection>
	{
		public MyServer()
		{
			// Configure the server
			this.Configuration.Backlog = 100;
            		this.Configuration.Port = 8888;
            		this.Configuration.MaximumNumberOfConnections = 100;
            		this.Configuration.Host = "127.0.0.1";
		}

        	protected override void Initialize()
        	{
            		Console.WriteLine("Server is ready.");
        	}

        	protected override void OnClientConnected(ClientConnection connection)
        	{
            		Console.WriteLine("New client connected!");

           		connection.SendFirstPacket();
        	}

        	protected override void OnClientDisconnected(ClientConnection connection)
        	{
            		Console.WriteLine("Client disconnected!");
        	}
	}

	class ClientConnection : NetConnection
	{
		public void SendFirstPacket()
		{
		    	using (var packet = new NetPacket())
		    	{
				packet.Write("Welcome " + this.Id.ToString());

				this.Send(packet);
		    	}
		}

		public override void HandleMessage(NetPacketBase packet)
		{
		    	string value = packet.Read<string>();

		    	Console.WriteLine("Received '{1}' from {0}", this.Id, value);

		    	using (var p = new NetPacket())
		    	{
				p.Write(string.Format("OK: '{0}'", value));
				this.Send(p);
		    	}
		}
    	}
}
```

### Client application

```c#
using System;
using Ether.Network;
using Ether.Network.Packets;

namespace SampleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new MyClient("127.0.0.1", 8888, 4096);
            client.Connect();
            Console.WriteLine("Enter a message and press enter...");

            while (true)
            {
                string input = Console.ReadLine();

                if (input == "quit") break;

                using (var packet = new NetPacket())
                {
                    packet.Write(input);
                    client.Send(packet);
                }
            }

            client.Disconnect();

            Console.WriteLine("Disconnected. Press any key to continue...");
            Console.ReadLine();
        }
    }

    class MyClient : NetClient
    {
        public MyClient(string host, int port, int bufferSize) 
            : base(host, port, bufferSize)
        {
        }

        protected override void HandleMessage(NetPacketBase packet)
        {
            var response = packet.Read<string>();

            Console.WriteLine("-> Server response: {0}", response);
        }

        protected override void OnConnected()
        {
            Console.WriteLine("Connected to {0}", this.Socket.RemoteEndPoint.ToString());
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine("Disconnected");
        }
    }
}
```
