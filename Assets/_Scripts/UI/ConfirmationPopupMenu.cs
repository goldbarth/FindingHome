using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class ConfirmationPopupMenu : Menu
    {
        [Header("COMPONENTS")]  
        [SerializeField] private TextMeshProUGUI displayText;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;
        
        //TODO: Need to use more UnityEvents, Actions, and Delegates for more modular programming.
        //NOTE: 
        public void ActivateMenu(string displayedText, UnityAction confirmAction, UnityAction cancelAction)
        {
            gameObject.SetActive(true);
            displayText.text = displayedText;
            
            // remove previous listeners if any exist.
            confirmButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();
            
            // create event and assign new listeners
            confirmButton.onClick.AddListener(() =>
            {
                DeactivateMenu();
                confirmAction();
            });
            cancelButton.onClick.AddListener(() =>
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
