using System;
using DataPersistence;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MainMenu
{
    //TODO: work on main menu and add more options
    //TODO: create a scene loader?
    public enum MenuState
    {
        Game,
        MainMenu,
    }
    
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueGameButton;
        
        public MenuState menuState;

        private void Start()
        {
            // Check if there is a save file and enable the continue button if there is
            if (!DataPersistenceManager.Instance.HasGameData())
                continueGameButton.interactable = false;
        }

        public void OnNewGameClicked()
        {
            DisableMenuButtons();
            
            // create a new game - init the data
            DataPersistenceManager.Instance.NewGame();
            
            // load the game scene - which in turn will save the game
            // in the DataPersistenceManager due to OnSceneUnLoaded.
            SceneManager.LoadSceneAsync((int)MenuState.Game);
        }
        
        public void OnContinueGameClicked()
        {
            DisableMenuButtons();
            
            // load the next scene - which will turn load the game
            // because of OnSceneLoaded in DataPersistenceManager
            SceneManager.LoadSceneAsync("SampleScene");
        }
        
        public void OnOptionsClicked()
        {
            Debug.Log("Options Clicked");
        }
        
        public void OnExitClicked()
        {
            Debug.Log("Exit Clicked");
        }
        
        private void DisableMenuButtons()
        {
            newGameButton.interactable = false;
            continueGameButton.interactable = false;
        }
    }
}
