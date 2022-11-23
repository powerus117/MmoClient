using ModestTree;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class UIElementSelector : MonoBehaviour
    {
        [SerializeField]
        private Selectable _preSelected;

        [SerializeField]
        private Selectable[] _selectables;

        [SerializeField]
        private Button _submitButtonClick;

        private int _selectedIndex;

        private void OnEnable()
        {
            if (_preSelected != null)
            {
                _preSelected.Select();

                int index = _selectables.IndexOf(_preSelected);

                if (index >= 0)
                {
                    _selectedIndex = index;
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                _selectedIndex = (_selectedIndex + 1) % _selectables.Length;
                
                _selectables[_selectedIndex].Select();
            }

            if (Input.GetKeyDown(KeyCode.Return) && _submitButtonClick != null)
            {
                _submitButtonClick.onClick.Invoke();
            }
        }
    }
}