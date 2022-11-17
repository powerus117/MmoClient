using UnityEngine;

namespace Core.Connection
{
    [CreateAssetMenu(fileName = "ConnectionConfig", menuName = "MMO/Core/ConnectionConfig")]
    public class ConnectionConfig : ScriptableObject
    {
        [SerializeField]
        private string _serverIp = "localhost";

        [SerializeField]
        private int _port = 7800;

        public string ServerIp => _serverIp;
        public int Port => _port;
    }
}