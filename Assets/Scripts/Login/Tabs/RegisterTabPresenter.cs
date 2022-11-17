using System;
using Core.Connection;
using Login.Dialogs;
using MmoShared.Messages.Login.Register;
using Services.Login;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Login.Tabs
{
    public class RegisterTabPresenter : MonoBehaviour
    {
        public event Action BackClicked;

        [SerializeField]
        private TMP_InputField _usernameInputField;

        [SerializeField]
        private TMP_InputField _passwordInputField;
        
        [SerializeField]
        private TMP_InputField _passwordRepeatInputField;
        
        [SerializeField]
        private Button _registerButton;

        [SerializeField]
        private Button _backButton;

        [Inject]
        private LoginDialogPresenter _dialogPresenter;

        [Inject]
        private ILoginService _loginService;

        [Inject]
        private IConnectionManager _connectionManager;

        private readonly CompositeDisposable _viewSubscriptions = new CompositeDisposable();
        
        private void Awake()
        {
            _registerButton.OnClickAsObservable().Subscribe(_ => OnRegisterClicked()).AddTo(_viewSubscriptions);
            _backButton.OnClickAsObservable().Subscribe(_ => OnBackClicked()).AddTo(_viewSubscriptions);
        }

        private void OnDestroy()
        {
            _viewSubscriptions.Dispose();
        }

        private async void OnRegisterClicked()
        {
            if (!_passwordInputField.text.Equals(_passwordRepeatInputField.text))
            {
                _dialogPresenter.DisplayText("Passwords do not match");
                return;
            }

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
            
            _dialogPresenter.DisplayText("Registering...", true);
            var result = await _loginService.Register(_usernameInputField.text, _passwordInputField.text);
            
            _dialogPresenter.DisplayText(result.ResultCode == RegisterResultCode.Success ? "Register successful!" :
                "Register failed with error: " + result.ResultCode);
        }
        
        private void OnBackClicked()
        {
            BackClicked?.Invoke();
        }
    }
}