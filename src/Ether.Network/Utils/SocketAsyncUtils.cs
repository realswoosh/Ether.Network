using Ether.Network.Interfaces;
using System;
using System.Linq;
using System.Net.Sockets;

namespace Ether.Network.Utils
{
    internal static class SocketAsyncUtils
    {
        public static void ProcessReceivedData(SocketAsyncEventArgs e, IAsyncUserToken token, IPacketProcessor packetProcessor, int alreadyProcessedDataSize)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            int dataStartOffset = token.DataStartOffset;

            if (alreadyProcessedDataSize >= token.TotalReceivedDataSize)
                return;

            if (!token.MessageSize.HasValue)
            {
                Console.WriteLine("Read message size");
                int headerSize = packetProcessor.HeaderSize;

                if (token.ReceivedHeaderBytesCount == headerSize)
                {
                    int messageSize = packetProcessor.GetLength(token.MessageHeaderData);

                    token.MessageSize = messageSize - headerSize;
                    token.MessageHeaderData = null;
                    token.ReceivedHeaderBytesCount = 0;

                    ProcessReceivedData(e, token, packetProcessor, alreadyProcessedDataSize);
                }
                else if (token.TotalReceivedDataSize > headerSize && token.MessageHeaderData == null)
                {
                    token.MessageHeaderData = NetUtils.GetPacketBuffer(e.Buffer, dataStartOffset, headerSize);
                    token.DataStartOffset = dataStartOffset + headerSize;
                    token.ReceivedHeaderBytesCount = headerSize;

                    ProcessReceivedData(e, token, packetProcessor, alreadyProcessedDataSize + headerSize);
                }
                else if (token.ReceivedHeaderBytesCount < headerSize)
                {
                    int rest = Math.Min(e.Buffer.Length - dataStartOffset, headerSize - token.ReceivedHeaderBytesCount);
                    byte[] remainingBuffer = NetUtils.GetPacketBuffer(e.Buffer, dataStartOffset, rest);

                    token.MessageHeaderData = token.MessageHeaderData == null ?
                        remainingBuffer : token.MessageHeaderData.Concat(remainingBuffer).ToArray();

                    token.ReceivedHeaderBytesCount += remainingBuffer.Length;
                    token.DataStartOffset += remainingBuffer.Length;

                    ProcessReceivedData(e, token, packetProcessor, alreadyProcessedDataSize + rest);
                }
            }
            else
            {
                var messageSize = token.MessageSize.Value;

                if (token.TotalReceivedDataSize - alreadyProcessedDataSize >= messageSize && token.MessageData == null)
                {
                    token.MessageData = NetUtils.GetPacketBuffer(e.Buffer, dataStartOffset, messageSize);
                    token.DataStartOffset = dataStartOffset + messageSize;
                    token.MessageSize = null;
                    token.ReceivedMessageBytesCount = messageSize;
                    alreadyProcessedDataSize += messageSize;

                    //ProcessReceivedData(e, token, packetProcessor, alreadyProcessedDataSize + messageSize);
                }
                else if (token.ReceivedMessageBytesCount < messageSize)
                {
                    int rest = Math.Min(e.Buffer.Length - dataStartOffset, messageSize - token.ReceivedMessageBytesCount);
                    byte[] remainingBuffer = NetUtils.GetPacketBuffer(e.Buffer, dataStartOffset, rest);

                    token.MessageData = token.MessageData == null ?
                        remainingBuffer : token.MessageData.Concat(remainingBuffer).ToArray();

                    token.ReceivedMessageBytesCount += remainingBuffer.Length;
                    token.DataStartOffset += remainingBuffer.Length;
                    alreadyProcessedDataSize += rest;

                    //ProcessReceivedData(e, token, packetProcessor, alreadyProcessedDataSize + rest);
                }

                if (token.ReceivedMessageBytesCount == messageSize && token.MessageData != null)
                {
                    token.MessageHandler?.Invoke(token.MessageData);
                    token.ReceivedMessageBytesCount = 0;
                    token.MessageData = null;
                    token.MessageSize = null;
                }

                ProcessReceivedData(e, token, packetProcessor, alreadyProcessedDataSize);
            }
        }

        public static void ProcessNextReceive(SocketAsyncEventArgs e, IAsyncUserToken token)
        {
            token.NextReceiveOffset += e.BytesTransferred;

            if (token.NextReceiveOffset == e.Buffer.Length)
            {
                token.NextReceiveOffset = 0;

                if (token.DataStartOffset < e.Buffer.Length)
                {
                    var notYesProcessDataSize = e.Buffer.Length - token.DataStartOffset;
                    Buffer.BlockCopy(e.Buffer, token.DataStartOffset, e.Buffer, 0, notYesProcessDataSize);

                    token.NextReceiveOffset = notYesProcessDataSize;
                }

                token.DataStartOffset = 0;
            }

            e.SetBuffer(token.NextReceiveOffset, e.Buffer.Length - token.NextReceiveOffset);
        }
    }
}
