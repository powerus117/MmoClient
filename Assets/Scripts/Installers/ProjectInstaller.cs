using Core.Connection;
using Core.Connection.Messages;
using Core.SceneLoading;
using Zenject;

namespace Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            InstallConnection();
            Container.BindInterfacesAndSelfTo<SceneLoader>().AsSingle().NonLazy();
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
