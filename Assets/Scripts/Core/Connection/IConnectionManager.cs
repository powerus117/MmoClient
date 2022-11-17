using System.Threading.Tasks;
using MmoShared.Messages;

namespace Core.Connection
{
    public interface IConnectionManager
    {
        bool IsConnected { get; }
        Task Connect();
        void SendMessage(Message message);
    }
}