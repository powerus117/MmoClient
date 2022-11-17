using System.Threading.Tasks;
using MmoShared.Messages.Login;
using MmoShared.Messages.Login.Register;

namespace Services.Login
{
    public interface ILoginService
    {
        Task<LoginResultSync> Login(string username, string password);
        Task<RegisterResultSync> Register(string username, string password);
    }
}