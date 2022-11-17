using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Login.Dialogs
{
    public class LoginDialogPresenter : MonoBehaviour
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
            _closeButton.OnClickAsObservable().Subscribe(_ => OnCloseClicked()).AddTo(_viewSubscriptions);
        }

        private void OnDestroy()
        {
            _viewSubscriptions.Dispose();
        }

        private void OnCloseClicked()
        {
            gameObject.SetActive(false);
        }

        public void DisplayText(string text, bool loadingIcon = false)
        {
            _text.text = text;
            
            //_loadingContainer.gameObject.SetActive(loadingIcon);
            gameObject.SetActive(true);
        }
    }
}