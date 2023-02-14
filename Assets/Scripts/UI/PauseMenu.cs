using DataPersistence;
using UnityEngine.UI;
using UnityEngine;

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
            //TODO: integrate with scene loader
            GameManager.Instance.IsPaused = false;
            SceneLoader.Instance.LoadSceneAsync(SceneIndex.Game);
        }
        
        public void OnOptionsMenuClicked()
        {
            //TODO: integrate with scene loader
            SceneLoader.Instance.LoadSceneAsync(SceneIndex.Options);
        }
        
        public void OnSaveGameClicked()
        {
            DataPersistenceManager.Instance.SaveGame();
        }
        
        public void OnLoadGameClicked()
        {
            SceneLoader.Instance.LoadSceneAsync(SceneIndex.LoadMenu);
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