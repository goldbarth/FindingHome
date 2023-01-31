using DataPersistence;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainMenu : Menu
    {
        [Header("MENU NAVIGATION")] 
        [SerializeField] private SaveSlotsMenu saveSlotsMenu;
        
        [Space][Header("MENU BUTTONS")]
        [SerializeField] private Button newGameButton;
        [Space][SerializeField] private Button continueGameButton;
        [Space][SerializeField] private Button loadGameButton;
        [Space][SerializeField] private Button optionsButton;
        [Space][SerializeField] private Button quitButton;

        private void Start()
        {
            DisableButtonsDependingOnData();
        }

        public void DisableButtonsDependingOnData()
        {
            // Check if there is a save file and enable the continue button if there is
            if (DataPersistenceManager.Instance.HasGameData()) return;
            continueGameButton.interactable = false;
            loadGameButton.interactable = false;
        }

        #region Button Clicks

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
            //TODO: integrate with scene loader
            SceneLoader.Instance.LoadScene(SceneIndex.Game);
            //SceneManager.LoadSceneAsync((int)SceneIndex.Game);
            
        }
        
        public void OnOptionsClicked()
        {
            SceneLoader.Instance.LoadScene(SceneIndex.Options);
        }
        
        public void OnExitClicked()
        {
            Application.Quit();
        }
        
        #endregion
        
        private void DisableMenuButtons()
        {
            newGameButton.interactable = false;
            continueGameButton.interactable = false;
            loadGameButton.interactable = false;
            optionsButton.interactable = false;
            quitButton.interactable = false;
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