using Login;
using Login.Tabs;
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

        public override void InstallBindings()
        {
            Container.Bind().FromInstance(_loginScreenPresenter);
            Container.Bind().FromInstance(_loginTabPresenter);
            Container.Bind().FromInstance(_registerTabPresenter);
        }
    }
}
