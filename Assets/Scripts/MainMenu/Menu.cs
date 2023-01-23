using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class Menu : MonoBehaviour
    {
        [Header("FIRST SELECTED BUTTON")]
        [SerializeField] private Button firstSelected;

        protected virtual void OnEnable()
        {
            SetFirstSelected(firstSelected);
        }

        // prevents after scene change, that the first selected button in the next scene is not selectable
        protected static void SetFirstSelected(Button firstSelectedButton)
        {
            firstSelectedButton.Select();
        }
    }
}