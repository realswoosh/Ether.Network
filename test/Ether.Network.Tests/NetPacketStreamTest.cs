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
        private static readonly byte[] ByteArray = new byte[] { ByteValue };
        private static readonly byte[] ShortArray = BitConverter.GetBytes(ShortValue);
        private static readonly byte[] Int32Array = BitConverter.GetBytes(Int32Value);
        private static readonly byte[] Int64Array = BitConverter.GetBytes(Int64Value);
        private static readonly byte[] StringByteArray = Encoding.ASCII.GetBytes(StringValue);

        [Fact]
        public void ReadByte()
        {
            this.TestRead<byte>(ByteArray, ByteValue);
        }

        [Fact]
        public void ReadInt16()
        {
            this.TestRead<short>(ShortArray, ShortValue);
        }

        [Fact]
        public void ReadInt32()
        {
            this.TestRead<int>(Int32Array, Int32Value);
        }

        [Fact]
        public void ReadInt64()
        {
            this.TestRead<long>(Int64Array, Int64Value);
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

        [Fact]
        public void WriteByte()
        {
            this.TestWrite<byte>(ByteArray, ByteValue);
        }

        [Fact]
        public void WriteShort()
        {
            this.TestWrite<short>(ShortArray, ShortValue);
        }

        [Fact]
        public void WriteInt32()
        {
            this.TestWrite<int>(Int32Array, Int32Value);
        }

        [Fact]
        public void WriteLong()
        {
            this.TestWrite<long>(Int64Array, Int64Value);
        }

        [Fact]
        public void WriteString()
        {
            string readString = null;
            byte[] packetStreamBuffer = null;

            using (INetPacketStream packetStream = new NetPacketStream())
            {
                packetStream.Write(StringValue);
                packetStreamBuffer = packetStream.Buffer;
            }

            using (INetPacketStream readPacketStream = new NetPacketStream(packetStreamBuffer))
            {
                readString = readPacketStream.Read<string>();
            }

            Assert.Equal(StringValue, readString);
        }

        private void TestRead<T>(byte[] input, T expected)
        {
            T value = default(T);

            using (INetPacketStream packetStream = new NetPacketStream(input))
                value = packetStream.Read<T>();

            Assert.Equal(expected, value);
        }

        private void TestWrite<T>(byte[] expected, T value)
        {
            byte[] packetStreamBuffer = null;

            using (INetPacketStream packetStream = new NetPacketStream())
            {
                packetStream.Write(value);
                packetStreamBuffer = packetStream.Buffer;
            }

            Assert.Equal(expected, packetStreamBuffer);
        }
    }
}
