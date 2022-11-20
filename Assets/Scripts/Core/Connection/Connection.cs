using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using Core.Connection.Messages;
using MmoShared.Messages;
using Newtonsoft.Json;
using ProtoBuf;
using ProtoBuf.Meta;
using UnityEngine;

namespace Core.Connection
{
    public class Connection
    {
        private const int MaxMessagesPerCycle = 5;
        
        private readonly TcpClient _client;
        private readonly NetworkStream _networkStream;
        private readonly BinaryReader _binaryReader;
        private readonly BinaryWriter _binaryWriter;
        private readonly IMessageReceiver _messageReceiver;
        
        private readonly ConcurrentQueue<Message> _incomingMessageQueue = new ConcurrentQueue<Message>();

        public bool IsConnected => _client.Connected;
        
        public Connection(TcpClient client, IMessageReceiver messageReceiver)
        {
            _client = client;
            _messageReceiver = messageReceiver;
            _networkStream = _client.GetStream();
            _binaryReader = new BinaryReader(_networkStream);
            _binaryWriter = new BinaryWriter(_networkStream);
        }

        public void ReadMessages()
        {
            if (!IsConnected)
            {
                return;
            }
            
            for (int i = 0; i < MaxMessagesPerCycle; i++)
            {
                if (_client.Available <= 0)
                {
                    // Nothing to read
                    break;
                }

                try
                {
                    // Read first uint as message id
                    MessageId msgId = (MessageId)_binaryReader.ReadUInt16();

                    if (MessageTypeHelper.IdToTypeMap.TryGetValue(msgId, out var messageType))
                    {
                        var deserializedMessage = RuntimeTypeModel.Default.DeserializeWithLengthPrefix(_networkStream, null, messageType,
                            PrefixStyle.Base128, 0);

                        if (deserializedMessage == null)
                        {
                            throw new NullReferenceException();
                        }

                        Message readMessage = (Message)Convert.ChangeType(deserializedMessage, messageType);
                        _incomingMessageQueue.Enqueue(readMessage);
                        
                        Debug.Log($"Received message {msgId} {JsonConvert.SerializeObject(readMessage)}");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    _client.Close();
                }
            }
        }

        public void ProcessMessages()
        {
            if (!IsConnected)
            {
                return;
            }

            for (int i = 0; i < MaxMessagesPerCycle && _incomingMessageQueue.TryDequeue(out var message); i++)
            {
                try
                {
                    _messageReceiver.Send(message);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        public void AddMessage(Message message)
        {
            if (!IsConnected)
            {
                return;
            }
            
            try
            {
                if (MessageTypeHelper.IdToTypeMap.TryGetValue(message.Id, out var messageTypeInfo))
                {
                    _binaryWriter.Write((ushort)message.Id);

                    RuntimeTypeModel.Default.SerializeWithLengthPrefix(_networkStream, message, message.GetType(),
                        PrefixStyle.Base128, 0);

                    Debug.Log($"Sent message {message.Id}: {JsonConvert.SerializeObject(message)}");
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                _client.Close();
            }
        }
        
        public void Close()
        {
            _binaryWriter.Dispose();
            _binaryReader.Dispose();
            _networkStream.Close();
            _client.Close();
        }
    }
}