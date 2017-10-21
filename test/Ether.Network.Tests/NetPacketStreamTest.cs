using Ether.Network.Core;
using Ether.Network.Packets;
using Ether.Network.Tests.Helpers;
using System;
using System.Text;
using Xunit;

namespace Ether.Network.Tests
{
    public class NetPacketStreamTest
    {
        private static readonly byte ByteValue = 145;
        private static readonly short ShortValue = 30546;
        private static readonly int Int32Value = 452674652;
        private static readonly long Int64Value = 3465479740298342;
        private static readonly string StringValue = Helper.GenerateRandomString(543);
        private static readonly byte[] ByteArray = BitConverter.GetBytes(ByteValue);
        private static readonly byte[] ShortArray = BitConverter.GetBytes(ShortValue);
        private static readonly byte[] Int32Array = BitConverter.GetBytes(Int32Value);
        private static readonly byte[] Int64Array = BitConverter.GetBytes(Int64Value);
        private static readonly byte[] StringByteArray = Encoding.ASCII.GetBytes(StringValue);

        [Fact]
        public void ReadByte()
        {
            byte value = 0;

            using (INetPacketStream packetStream = new NetPacketStream(ByteArray))
                value = packetStream.Read<byte>();

            Assert.Equal(ByteValue, value);
        }

        [Fact]
        public void ReadInt16()
        {
            short value = 0;

            using (INetPacketStream packetStream = new NetPacketStream(ShortArray))
                value = packetStream.Read<short>();

            Assert.Equal(ShortValue, value);
        }

        [Fact]
        public void ReadInt32()
        {
            int value = 0;

            using (INetPacketStream packetStream = new NetPacketStream(Int32Array))
                value = packetStream.Read<int>();

            Assert.Equal(Int32Value, value);
        }

        [Fact]
        public void ReadInt64()
        {
            long value = 0;

            using (INetPacketStream packetStream = new NetPacketStream(Int64Array))
                value = packetStream.Read<long>();

            Assert.Equal(Int64Value, value);
        }

        [Fact]
        public void ReadByteArray()
        {
            byte[] value = null;

            using (INetPacketStream packetStream = new NetPacketStream(StringByteArray))
                value = packetStream.Read<byte>(StringByteArray.Length);

            string convertedValue = Encoding.ASCII.GetString(value);

            Assert.Equal(StringValue, convertedValue);
        }
    }
}
