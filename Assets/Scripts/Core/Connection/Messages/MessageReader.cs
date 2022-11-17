using System;
using System.Threading;
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
        }

        private void ReadThread()
        {
            try
            {
                while (_connection.IsConnected)
                {
                    _connection.ReadMessages();
                    
                    _connection.ProcessMessages();
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