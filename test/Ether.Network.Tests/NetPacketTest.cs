using Ether.Network.Helpers;
using Ether.Network.Packets;
using Ether.Network.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ether.Network.Tests
{
    public class NetPacketTest
    {
        [Fact]
        public void NetPacketCorrectLength()
        {
            var packet = new NetPacket();

            packet.Write(42);
            packet.Write("Hello world!");
            packet.Write("This is a NetPacket");

            var packetBuffer = packet.Buffer;

            Assert.Equal(packetBuffer.Length, packet.Size);
        }

        [Fact]
        public void NetPacketIncorrectLength()
        {
            int packetSize = 0;

            using (var packet = new NetPacket())
            {
                packet.Write(42);
                packet.Write<ushort>(2);
                packetSize = packet.Buffer.Length;
            }

            Assert.NotEqual(packetSize, 6);
        }

        [Fact]
        public void NetPacketReadInt32()
        {
            byte[] buffer = BitConverter.GetBytes(22);
            int value = 0;

            using (var packet = new NetPacket(buffer))
                value = packet.Read<int>();

            Assert.Equal(22, value);
        }

        [Fact]
        public void NetPacketReadStringSuccess()
        {
            string text = Helper.GenerateRandomString();
            byte[] buffer = null;
            string readText = null;

            using (var writePacket = new NetPacket())
            {
                writePacket.Write(text);
                buffer = writePacket.Buffer;
            }

            using (var readPacket = new NetPacket(buffer))
            {
                readPacket.Read<int>(); // mandatory: read packet size when not using NetPacket.Split method.
                readText = readPacket.Read<string>();
            }
            
            Assert.Equal(text, readText);
        }

        [Fact]
        public void NetPacketReadStringFailure()
        {
            string text = Helper.GenerateRandomString();
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            string readText = null;

            try
            {
                // don't read the packet size must fail
                using (var packet = new NetPacket(buffer))
                    readText = packet.Read<string>();
            }
            catch { }
            finally
            {
                Assert.NotEqual(readText, text);
            }
        }
    }
}
