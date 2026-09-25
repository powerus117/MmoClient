using System.Threading.Tasks;
using MmoShared.Messages.Login;
using MmoShared.Messages.Login.Register;
using MmoShared.Messages.Players.Domain;

namespace Services.Login
{
    public interface ILoginService
    {
        PlayerDataDto PlayerDataDto { get; }
        Task<LoginResultSync> Login(string username, string password);
        Task<RegisterResultSync> Register(string username, string password);
    }
}