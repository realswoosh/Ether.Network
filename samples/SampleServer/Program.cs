using Ether.Network;
using System;

namespace SampleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }

    class SampleServer : NetServer<Client>
    {
    }

    class Client : NetConnection
    {
    }
}