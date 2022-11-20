using Core.Connection;
using Core.Connection.Messages;
using Core.SceneLoading;
using Core.Signals;
using Services.Login;
using Services.Players;
using Zenject;

namespace Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            InstallConnection();
            
            // Core
            Container.BindInterfacesAndSelfTo<SceneLoader>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<SignalManager>().AsSingle().NonLazy();
            
            Container.BindInterfacesAndSelfTo<PlayerService>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LoginService>().AsSingle().NonLazy();
        }

        private void InstallConnection()
        {
            Container.Bind<ConnectionConfig>().FromResource("Configs/Connection/ConnectionConfig");
            Container.BindInterfacesAndSelfTo<ConnectionManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<MessageReceiver>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<MessageSender>().AsSingle().NonLazy();
        }
    }
}
