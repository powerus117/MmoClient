using Login;
using Login.Dialogs;
using Login.Tabs;
using Services.Login;
using UnityEngine;
using Zenject;

namespace Installers.Login
{
    public class LoginInstaller : MonoInstaller
    {
        [SerializeField]
        private LoginScreenPresenter _loginScreenPresenter;

        [SerializeField]
        private LoginTabPresenter _loginTabPresenter;

        [SerializeField]
        private RegisterTabPresenter _registerTabPresenter;

        [SerializeField]
        private LoginDialogPresenter _loginDialogPresenter;
        
        public override void InstallBindings()
        {
            Container.Bind().FromInstance(_loginScreenPresenter);
            Container.Bind().FromInstance(_loginTabPresenter);
            Container.Bind().FromInstance(_registerTabPresenter);
            Container.Bind<LoginDialogPresenter>().FromInstance(_loginDialogPresenter);

            Container.BindInterfacesAndSelfTo<LoginService>().AsSingle().NonLazy();
        }
    }
}