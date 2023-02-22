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
        
        public void ActivateMenu(string displayedText, UnityAction confirmAction, UnityAction cancelAction)
        {
            this.gameObject.SetActive(true);
            this.displayText.text = displayedText;
            
            // remove previous listeners if any exist. only removes listeners added through code
            confirmButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();
            
            // assign new listeners
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
