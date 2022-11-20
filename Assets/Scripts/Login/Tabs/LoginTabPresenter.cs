using System;
using Core.Connection;
using Core.Signals;
using Login.Dialogs;
using Login.Signals;
using MmoShared.Messages.Login;
using Services.Login;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Login.Tabs
{
    public class LoginTabPresenter : MonoBehaviour
    {
        public event Action RegisterClicked;

        [SerializeField]
        private TMP_InputField _usernameInputField;

        [SerializeField]
        private TMP_InputField _passwordInputField;
        
        [SerializeField]
        private Button _loginButton;

        [SerializeField]
        private Button _registerButton;
        
        [Inject]
        private LoginDialogPresenter _dialogPresenter;
        
        [Inject]
        private ILoginService _loginService;

        [Inject]
        private IConnectionManager _connectionManager;

        [Inject]
        private ISignalManager _signalManager;

        private readonly CompositeDisposable _viewSubscriptions = new CompositeDisposable();
        
        private void Awake()
        {
            _loginButton.OnClickAsObservable().Subscribe(_ => OnLoginClicked()).AddTo(_viewSubscriptions);
            _registerButton.OnClickAsObservable().Subscribe(_ => OnRegisterClicked()).AddTo(_viewSubscriptions);
        }

        private void OnDestroy()
        {
            _viewSubscriptions.Dispose();
        }

        private async void OnLoginClicked()
        {
            if (!_connectionManager.IsConnected)
            {
                _dialogPresenter.DisplayText("Connecting...", true);
                await _connectionManager.Connect();

                if (!_connectionManager.IsConnected)
                {
                    _dialogPresenter.DisplayText("Connection failed");
                    return;
                }
            }
            
            _dialogPresenter.DisplayText("Logging in...", true);
            var result = await _loginService.Login(_usernameInputField.text, _passwordInputField.text);
            
            _dialogPresenter.DisplayText(result.ResultCode == LoginResultCode.Success ? "Login successful!" :
                "Login failed with error: " + result.ResultCode);

            if (result.ResultCode == LoginResultCode.Success)
            {
                _signalManager.Send(new LoggedInSignal());
            }
        }
        
        private void OnRegisterClicked()
        {
            RegisterClicked?.Invoke();
        }
    }
}