using DataPersistence;
using SceneHandler;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class PauseMenu : Menu
    {
        [Space][Header("MENU BUTTONS")]
        [Space][SerializeField] private Button resumeGameButton;
        [Space][SerializeField] private Button loadGameButton;
        [Space][Header("BUTTON LAYOUT")]
        [Space][SerializeField] private GameObject buttonLayout;
        
        private void Awake()
        {
            Time.timeScale = 0f;
            GameManager.Instance.IsPauseMenuActive = true;
            GameManager.Instance.IsGamePaused = true;
            DisableButtonDependingOnData();
        }

        private void Update()
        {
            buttonLayout.SetActive(GameManager.Instance.IsPauseMenuActive);
            if(GameManager.Instance.IsPauseMenuActive && !GameManager.Instance.IsSelected)
            { 
                resumeGameButton.Select();
                GameManager.Instance.IsSelected = true;
            }
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
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
            GameManager.Instance.IsGamePaused = false;
            SceneLoader.Instance.UnloadSceneAsync();
        }
        
        public void OnOptionsMenuClicked()
        {
            SceneLoader.Instance.LoadSceneAsync(SceneIndices.OptionsMenu, LoadSceneMode.Additive);
        }
        
        public void OnSaveGameClicked()
        {
            //DataPersistenceManager.Instance.ShowSaveAnimation = true;
            DataPersistenceManager.Instance.SaveGame();
        }
        
        public void OnLoadGameClicked()
        {
            SceneLoader.Instance.LoadSceneAsync(SceneIndices.LoadMenu, LoadSceneMode.Additive);
        }
        
        public void OnMainMenuClicked()
        {
            GameManager.Instance.IsGamePaused = false;
            SceneLoader.Instance.LoadSceneAsync(SceneIndices.MainMenu);
        }
        
        public void OnQuitGameClicked()
        {
            Application.Quit();
        }
        
        #endregion
    }
}