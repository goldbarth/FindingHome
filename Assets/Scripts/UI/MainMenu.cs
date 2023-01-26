using DataPersistence;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    //TODO: work on main menu and add more options
    //TODO: create a scene loader?
    public enum MenuState
    {
        MainMenu,
        Game,
    }
    
    public class MainMenu : Menu
    {
        [Header("MENU NAVIGATION")] 
        [SerializeField] private SaveSlotsMenu saveSlotsMenu;
        
        [Space][Header("MENU BUTTONS")]
        [SerializeField] private Button newGameButton;
        [Space][SerializeField] private Button continueGameButton;
        [Space][SerializeField] private Button loadGameButton;
        
        [Space] public MenuState menuState;

        private void Start()
        {
            DisableButtonsDependingOnData();
        }

        public void DisableButtonsDependingOnData()
        {
            // Check if there is a save file and enable the continue button if there is
            if (!DataPersistenceManager.Instance.HasGameData())
            {
                continueGameButton.interactable = false;
                loadGameButton.interactable = false;
            }
        }

        public void OnNewGameClicked()
        {
            saveSlotsMenu.ActivateMenu(false);
            DeactivateMenu();
        }
        
        public void OnLoadGameClicked()
        {
            saveSlotsMenu.ActivateMenu(true);
            DeactivateMenu();
        }

        public void OnContinueGameClicked()
        {
            DisableMenuButtons();
            // save the game anytime before loading a new scene
            DataPersistenceManager.Instance.SaveGame();
            SceneManager.LoadSceneAsync((int)MenuState.Game);
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
        
        public void ActivateMenu()
        {
            gameObject.SetActive(true);
            DisableButtonsDependingOnData();
        }
        
        private void DeactivateMenu()
        {
            gameObject.SetActive(false);
        }
    }
}