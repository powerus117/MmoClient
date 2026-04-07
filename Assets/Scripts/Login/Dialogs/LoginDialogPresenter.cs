using Core.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Login.Dialogs
{
    public class LoginDialogPresenter : UIWindow<LoginDialogParams>
    {
        [SerializeField]
        private TMP_Text _text;

        [SerializeField]
        private Button _closeButton;

        [SerializeField]
        private GameObject _loadingContainer;

        private readonly CompositeDisposable _viewSubscriptions = new CompositeDisposable();

        private void Awake()
        {
            _closeButton.OnClickAsObservable().Subscribe(_ => Close()).AddTo(_viewSubscriptions);
        }

        private void OnDestroy()
        {
            _viewSubscriptions.Dispose();
        }

        protected override void OnParamsSet(LoginDialogParams parameters)
        {
            _text.text = parameters.Text;
            _loadingContainer.SetActive(parameters.ShowLoadingIcon);
        }
    }
}
