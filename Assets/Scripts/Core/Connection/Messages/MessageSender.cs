using MmoShared.Messages;
using UnityEngine;
using Zenject;

namespace Core.Connection.Messages
{
    public class MessageSender : IMessageSender
    {
        [Inject]
        private IConnectionManager _connectionManager;
        
        public void Send(Message message)
        {
            if (!_connectionManager.IsConnected)
            {
                Debug.LogError("Tried to send message while not being connected: " + message.Id);
                return;
            }
            
            _connectionManager.SendMessage(message);
        }
    }
}