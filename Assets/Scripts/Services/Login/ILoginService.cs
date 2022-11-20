using System.Threading.Tasks;
using MmoShared.Messages.Login;
using MmoShared.Messages.Login.Domain;
using MmoShared.Messages.Login.Register;

namespace Services.Login
{
    public interface ILoginService
    {
        UserInfo UserInfo { get; }
        Task<LoginResultSync> Login(string username, string password);
        Task<RegisterResultSync> Register(string username, string password);
    }
}