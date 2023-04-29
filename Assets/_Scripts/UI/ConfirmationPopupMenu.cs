using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class ConfirmationPopupMenu : Menu
    {
        [Header("COMPONENTS")]  
        [SerializeField] private TextMeshProUGUI _displayText;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;
        
        //TODO: Need to use more UnityEvents, Actions, and Delegates for more modular programming.
        //NOTE: 
        public void ActivateMenu(string displayedText, UnityAction confirmAction, UnityAction cancelAction)
        {
            gameObject.SetActive(true);
            _displayText.text = displayedText;
            
            // remove previous listeners if any exist.
            _confirmButton.onClick.RemoveAllListeners();
            _cancelButton.onClick.RemoveAllListeners();
            
            // create event and assign new listeners
            _confirmButton.onClick.AddListener(() =>
            {
                DeactivateMenu();
                confirmAction();
            });
            _cancelButton.onClick.AddListener(() =>
            {
                DeactivateMenu();
                cancelAction();
            });
        }

        private void DeactivateMenu()
        {
            gameObject.SetActive(false);
        }
    }
}
