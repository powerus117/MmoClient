using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Core.Connection.Messages;
using MmoShared.Messages;
using MmoShared.Messages.Core;
using UnityEngine;
using Zenject;

namespace Core.Connection
{
    public class ConnectionManager : IConnectionManager, IDisposable
    {
        [Inject]
        private ConnectionConfig _connectionConfig;

        [Inject]
        private IMessageReceiver _messageReceiver;

        private Connection _connection;
        private MessageReader _messageReader;
        private bool _isConnecting;
        private bool _isStopping;

        public bool IsConnected => _connection is { IsConnected: true };

        public void Dispose()
        {
            SendMessage(new QuitNotify());
            Close();
        }

        public async Task Connect()
        {
            _isStopping = false;
            _isConnecting = true;
            Debug.Log($"Starting connection to {_connectionConfig.ServerIp}:{_connectionConfig.Port}");

            var tcpClient = new TcpClient();

            try
            {
                await tcpClient.ConnectAsync(_connectionConfig.ServerIp, _connectionConfig.Port);
                _connection = new Connection(tcpClient, _messageReceiver);
                _messageReader = new MessageReader(_connection);
            }
            catch (SocketException e)
            {
                Debug.Log($"SocketException: {e}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception: {e}");
                Close();
            }
            finally
            {
                _isConnecting = false;
            }

            if (_isStopping)
            {
                Close();
                return;
            }

            if (_connection?.IsConnected ?? false)
            {
                Debug.Log("Connected");
            }
        }

        public void SendMessage(Message message)
        {
            _connection.AddMessage(message);
        }

        private void Close()
        {
            _isStopping = true;

            if (_connection != null)
            {
                _connection.Close();
                _connection = null;
                
                Debug.Log("Connection closed");
            }
        }
    }
}
