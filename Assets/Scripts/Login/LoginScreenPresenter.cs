using Core.SceneLoading;
using Core.Signals;
using Login.Domain;
using Login.Signals;
using Login.Tabs;
using UniRx;
using UnityEngine;
using Zenject;

namespace Login
{
    public class LoginScreenPresenter : MonoBehaviour
    {
        [SerializeField]
        private LoginTabPresenter _loginTabPresenter;

        [SerializeField]
        private RegisterTabPresenter _registerTabPresenter;

        [Inject]
        private ISignalManager _signalManager;

        [Inject]
        private ISceneLoader _sceneLoader;

        private readonly CompositeDisposable _modelSubscriptions = new CompositeDisposable();
        private LoginScreenModel _model = new LoginScreenModel();

        private void Awake()
        {
            _model.LoginScreenState.Subscribe(OnScreenStateChanged).AddTo(_modelSubscriptions);
            
            _loginTabPresenter.RegisterClicked += LoginTabPresenterOnRegisterClicked;
            _registerTabPresenter.BackClicked += RegisterTabPresenterOnBackClicked;
            
            _signalManager.Subscribe<LoggedInSignal>(OnLoggedInSignal);
        }

        private void OnDestroy()
        {
            _modelSubscriptions.Dispose();
            
            _loginTabPresenter.RegisterClicked -= LoginTabPresenterOnRegisterClicked;
            _registerTabPresenter.BackClicked -= RegisterTabPresenterOnBackClicked;
            
            _signalManager.Unsubscribe<LoggedInSignal>(OnLoggedInSignal);
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
        
        private void OnLoggedInSignal(LoggedInSignal signal)
        {
            _sceneLoader.LoadScene(SceneNames.Game);
        }
    }
}