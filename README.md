# Ether.Network 

[![Build Status](https://travis-ci.org/Eastrall/Ether.Network.svg?branch=develop)](https://travis-ci.org/Eastrall/Ether.Network)
[![NuGet Status](https://img.shields.io/nuget/v/Ether.Network.svg)](https://www.nuget.org/packages/Ether.Network/)

Ether.Network is a basic library to make quickly a simple server or client using sockets.

This library is coded in C# using .NET Core framework to target Windows and Linux operating systems.

For now we use the basic synchronous sockets, and in the future we'll add the support of asynchronous sockets to increase performances and stability.

## Framework support

- .NET Core 1.0
- .NET Framework 4.6.1
- .NET Framework 4.6
- .NET Framework 4.5.1
- .NET Framework 4.5

## Features

### Version 1.1.0

- [NetPacket][netpacket] : Packet abstraction. You can now create our own packet implementation.
- [NetDelayer][netdelayer] : Call actions every X milliseconds.
- [NetServer][netserver]: NetServer abstraction.
	- Custom treatments on your incoming data by overriding the `NetServer.SplitPackets` method.
	- OnClientConnected/Disconnected virtual methods. Can be overrided.

### Version 1.0.0

- [NetServer][netserver]: Socket server implementation handling multiple [Clients][netconnection]
- [NetClient][netclient]: Socket client implementation
- [NetPacket][netpacket]: Read/Write packet implementation.

## How to install

Create a .NETCore project and add the nuget package: `Ether.Network` or you can do it manually :

`Install-Package Ether.Network` in your package manager console.

## How to use

There is a sample server application for verison 1.1.0:

```c#
using Ether.Network;
using Ether.Network.Packets;
using System;
using System.Net.Sockets;

public class Program
{
    static void Main(string[] args)
    {
        using (var server = new MyServer())
            server.Start();
    }
}

public class MyServer : NetServer<Client>
{
    public MyServer()
    {
        this.ServerConfiguration = new NetConfiguration()
        {
            Ip = "127.0.0.1",
            Port = 5555
        };
    }

    public override void DisposeServer()
    {
        // Dispose the server unmanaged resources
    }

    protected override void Idle()
    {
        while (this.IsRunning) ;
    }

    protected override void Initialize()
    {
        // Initialize the server resources
    }

    protected override void OnClientConnected(Client client)
    {
        Console.WriteLine("New client connected with id: {0}", client.Id);
    }

    protected override void OnClientDisconnected(Client client)
    {
        Console.WriteLine("Client with id {0} disconnected.", client.Id);
    }
}

public class Client : NetConnection
{
    public Client()
    {
    }

    public Client(Socket acceptedSocket)
        : base(acceptedSocket)
    {
    }

    public override void Greetings()
    {
        // Send a packet to say hi to the incoming client
        using (var packet = new NetPacket())
        {
            packet.Write(42); // header
            packet.Write("Hello!");
            this.Send(packet);
        }
    }

    public override void HandleMessage(NetPacketBase packet)
    {
        // Handle incoming messages
        var header = packet.Read<int>();
        var message = packet.Read<string>();

        Console.WriteLine("Client {0} said: '{1}'", this.Id, message);
    }
}
```

And now a simple client app

```c#
using Ether.Network;
using Ether.Network.Packets;
using System;

public class MyClient : NetClient
{
    public MyClient()
    {
    }

    public override void HandleMessage(NetPacketBase packet)
    {
        // Handle incoming messages

        var header = packet.Read<int>();
        var message = packet.Read<string>();

        Console.WriteLine("I recieved the message: '{0}'", message);

        using (var newPacket = new NetPacket())
        {
            newPacket.Write(43);
            newPacket.Write("Hello world! This is a message from the client");

            this.Send(newPacket);
        }
    }

    protected override void OnClientDisconnected()
    {
    }
}
```

[netdelayer]: src/Ether.Network/NetDelayer.cs
[netserver]: src/Ether.Network/NetServer.cs
[netclient]: src/Ether.Network/NetClient.cs
[netpacket]: src/Ether.Network/Packets/NetPacket.cs
[netpacketbase]: src/Ether.Network/Packets/NetPacketBase.cs
[netconnection]: src/Ether.Network/NetConnection.cs
