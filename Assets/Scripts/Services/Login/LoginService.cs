using System.Threading.Tasks;
using Core.Connection.Messages;
using MmoShared.Messages.Login;
using MmoShared.Messages.Login.Register;
using Zenject;

namespace Services.Login
{
    public class LoginService : ILoginService
    {
        [Inject]
        private IMessageSender _messageSender;

        [Inject]
        private IMessageReceiver _messageReceiver;

        public async Task<LoginResultSync> Login(string username, string password)
        {
            LoginResultSync resultSync = null;
            
            void OnResult(LoginResultSync sync)
            {
                _messageReceiver.Unsubscribe<LoginResultSync>(OnResult);

                resultSync = sync;
            }
            
            _messageReceiver.Subscribe<LoginResultSync>(OnResult);

            //BCrypt.Net.BCrypt.HashPassword(password);
            
            LoginNotify loginNotify = new LoginNotify()
            {
                Username = username,
                Password = password
            };
            
            _messageSender.Send(loginNotify);

            while (resultSync == null)
            {
                await Task.Yield();
            }
            
            return resultSync;
        }

        public async Task<RegisterResultSync> Register(string username, string password)
        {
            RegisterResultSync resultSync = null;
            
            void OnResult(RegisterResultSync sync)
            {
                _messageReceiver.Unsubscribe<RegisterResultSync>(OnResult);

                resultSync = sync;
            }
            
            _messageReceiver.Subscribe<RegisterResultSync>(OnResult);
            
            RegisterNotify registerNotify = new RegisterNotify()
            {
                Username = username,
                Password = password
            };
            
            _messageSender.Send(registerNotify);

            while (resultSync == null)
            {
                await Task.Yield();
            }
            
            return resultSync;
        }
    }
}