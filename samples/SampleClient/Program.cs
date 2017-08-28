using System;
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
            int i = 0;

            while (i < 6)
            {
                string input = "That is a test " + i;

                if (input == "quit")
                {
                    break;
                }

                if (input != null)
                {
                    using (var packet = new NetPacket())
                    {
                        packet.Write(input);

                        client.Send(packet);
                    }
                }

                i++;
            }

            client.Disconnect();

            Console.WriteLine("Disconnected. Press any key to continue...");
            Console.ReadLine();
        }
    }
}