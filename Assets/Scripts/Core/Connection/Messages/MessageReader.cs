using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Connection.Messages
{
    public class MessageReader
    {
        private readonly Thread _thread;
        private readonly Connection _connection;

        public MessageReader(Connection connection)
        {
            _connection = connection;
            _thread = new Thread(ReadThread);
            _thread.Start();
            
            ProcessMessages();
        }

        private void ReadThread()
        {
            try
            {
                while (_connection.IsConnected)
                {
                    _connection.ReadMessages();
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        private async void ProcessMessages()
        {
            try
            {
                while (_connection.IsConnected)
                {
                    _connection.ProcessMessages();

                    await Task.Delay(10);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }
    }
}