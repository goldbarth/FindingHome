using DataPersistence;
using SceneHandler;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class PauseMenu : Menu
    {
        [Space][Header("MENU BUTTON")]
        [Space][SerializeField] private Button loadGameButton;
        
        private void Awake()
        {
            GameManager.Instance.IsPaused = true;
            DisableButtonDependingOnData();
        }

        private void DisableButtonDependingOnData()
        {
            // Check if there is a save file and enable the continue button if there is
            if (DataPersistenceManager.Instance.HasGameData()) return;
            loadGameButton.interactable = false;
        }

        #region Button Clicks
        
        public void OnResumeGameClicked()
        {
            GameManager.Instance.IsPaused = false;
            SceneLoader.Instance.UnloadSceneAsync();
        }
        
        public void OnOptionsMenuClicked()
        {
            SceneLoader.Instance.LoadSceneAsync(SceneIndex.OptionsMenu, LoadSceneMode.Additive);
        }
        
        public void OnSaveGameClicked()
        {
            DataPersistenceManager.Instance.SaveGame();
        }
        
        public void OnLoadGameClicked()
        {
            SceneLoader.Instance.LoadSceneAsync(SceneIndex.LoadMenu, LoadSceneMode.Additive);
        }
        
        public void OnMainMenuClicked()
        {
            SceneLoader.Instance.LoadSceneAsync(SceneIndex.MainMenu);
        }
        
        public void OnQuitGameClicked()
        {
            Application.Quit();
        }
        
        #endregion
    }
}