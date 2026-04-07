using System;
using Core.Connection;
using Core.Signals;
using Core.UI;
using Login.Dialogs;
using Login.Signals;
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
        private IUIService _uiService;

        [Inject]
        private ILoginService _loginService;

        [Inject]
        private IConnectionManager _connectionManager;

        [Inject]
        private ISignalManager _signalManager;

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
                await _uiService.Open<LoginDialogPresenter, LoginDialogParams>(
                    UIAddresses.LoginDialog,
                    new LoginDialogParams { Text = "Passwords do not match" },
                    UILayer.Dialog);
                return;
            }

            if (!_connectionManager.IsConnected)
            {
                await _uiService.Open<LoginDialogPresenter, LoginDialogParams>(
                    UIAddresses.LoginDialog,
                    new LoginDialogParams { Text = "Connecting...", ShowLoadingIcon = true },
                    UILayer.Dialog);

                await _connectionManager.Connect();

                if (!_connectionManager.IsConnected)
                {
                    await _uiService.Open<LoginDialogPresenter, LoginDialogParams>(
                        UIAddresses.LoginDialog,
                        new LoginDialogParams { Text = "Connection failed" },
                        UILayer.Dialog);
                    return;
                }
            }

            var dialog = await _uiService.Open<LoginDialogPresenter, LoginDialogParams>(
                UIAddresses.LoginDialog,
                new LoginDialogParams { Text = "Registering...", ShowLoadingIcon = true },
                UILayer.Dialog);
            var result = await _loginService.Register(_usernameInputField.text, _passwordInputField.text);

            dialog.SetParams(new LoginDialogParams
            {
                Text = result.ResultCode == RegisterResultCode.Success
                    ? "Register successful!"
                    : "Register failed with error: " + result.ResultCode
            });

            if (result.ResultCode == RegisterResultCode.Success)
            {
                _signalManager.Send(new LoggedInSignal());
            }
        }

        private void OnBackClicked()
        {
            BackClicked?.Invoke();
        }
    }
}
