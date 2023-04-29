using UnityEngine.UI;
using UnityEngine;

namespace UI
{
    public class Menu : MonoBehaviour
    {
        [Header("FIRST SELECTED BUTTON")]
        [SerializeField] private Button _firstSelected;

        protected virtual void OnEnable()
        {
            SetFirstSelected(_firstSelected);
        }

        // prevents after scene change, that the first selected button in the next scene is not selectable
        protected static void SetFirstSelected(Button firstSelectedButton)
        {
            firstSelectedButton.Select();
        }
        
    }
}