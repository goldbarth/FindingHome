using DataPersistence;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PauseMenu : Menu
    {
        [Header("MENU NAVIGATION")] 
        [SerializeField] private SaveSlotsMenu saveSlotsMenu;
        
        [Space][Header("MENU BUTTONS")]
        [SerializeField] private Button resumeGameButton;
        [Space][SerializeField] private Button saveGameButton;
        [Space][SerializeField] private Button loadGameButton;
        [Space][SerializeField] private Button optionsButton;
        [Space][SerializeField] private Button mainMenuButton;
        [Space][SerializeField] private Button quitButton;
        
        private void Awake()
        {
            GameManager.Instance.IsPaused = true;
            DisableButtonsDependingOnData();
        }

        private void OnDestroy()
        {
            GameManager.Instance.IsPaused = false;
        }

        public void DisableButtonsDependingOnData()
        {
            // Check if there is a save file and enable the continue button if there is
            if (DataPersistenceManager.Instance.HasGameData()) return;
            loadGameButton.interactable = false;
        }


        #region Button Clicks
        
        public void OnResumeGameClicked()
        {
            // save the game anytime before loading a new scene
            DataPersistenceManager.Instance.SaveGame();
            //TODO: integrate with scene loader
            SceneLoader.Instance.LoadScene(SceneIndex.Game);
        }
        
        public void OnOptionsMenuClicked()
        {
            //TODO: integrate with scene loader
            SceneLoader.Instance.LoadScene(SceneIndex.Options);
        }
        
        public void OnSaveGameClicked()
        {
            DataPersistenceManager.Instance.SaveGame();
        }
        
        public void OnLoadGameClicked()
        {
        }
        
        public void OnMainMenuClicked()
        {
            SceneLoader.Instance.LoadScene(SceneIndex.MainMenu);
        }
        
        public void OnQuitGameClicked()
        {
            Application.Quit();
        }
        
        #endregion
    }
}