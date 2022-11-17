using Login.Domain;
using Login.Tabs;
using UniRx;
using UnityEngine;

namespace Login
{
    public class LoginScreenPresenter : MonoBehaviour
    {
        [SerializeField]
        private LoginTabPresenter _loginTabPresenter;

        [SerializeField]
        private RegisterTabPresenter _registerTabPresenter;

        private readonly CompositeDisposable _modelSubscriptions = new CompositeDisposable();
        private LoginScreenModel _model = new LoginScreenModel();

        private void Awake()
        {
            _model.LoginScreenState.Subscribe(OnScreenStateChanged).AddTo(_modelSubscriptions);
            
            _loginTabPresenter.RegisterClicked += LoginTabPresenterOnRegisterClicked;
            _registerTabPresenter.BackClicked += RegisterTabPresenterOnBackClicked;
        }

        private void OnDestroy()
        {
            _modelSubscriptions.Dispose();
            
            _loginTabPresenter.RegisterClicked -= LoginTabPresenterOnRegisterClicked;
            _registerTabPresenter.BackClicked -= RegisterTabPresenterOnBackClicked;
        }
        
        private void OnScreenStateChanged(LoginScreenState loginScreenState)
        {
            _loginTabPresenter.gameObject.SetActive(loginScreenState == LoginScreenState.Login);
            _registerTabPresenter.gameObject.SetActive(loginScreenState == LoginScreenState.Register);
        }

        private void LoginTabPresenterOnRegisterClicked()
        {
            _model.LoginScreenState.Value = LoginScreenState.Register;
        }
        
        private void RegisterTabPresenterOnBackClicked()
        {
            _model.LoginScreenState.Value = LoginScreenState.Login;
        }
    }
}