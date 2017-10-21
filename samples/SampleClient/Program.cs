using System;
using Ether.Network.Packets;

namespace SampleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new MyClient("127.0.0.1", 4444, 512);
            client.Connect();

            Console.WriteLine("Enter a message and press enter...");
            int i = 0;

            try
            {
                while (true)
                {
                    string input = Console.ReadLine(); //"Yolo " + i.ToString();

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
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            
            client.Disconnect();

            Console.WriteLine("Disconnected. Press any key to continue...");
            Console.ReadLine();
        }
    }
}