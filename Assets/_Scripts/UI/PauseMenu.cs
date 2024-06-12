using UnityEngine.SceneManagement;
using DataPersistence;
using UnityEngine.UI;
using SceneHandler;
using UnityEngine;

namespace UI
{
    public class PauseMenu : Menu
    {
        [Space][Header("MENU BUTTONS")]
        [Space][SerializeField] private Button _resumeGameButton;
        // [Space][SerializeField] private Button _loadGameButton;
        [Space][Header("BUTTON LAYOUT")]
        [Space][SerializeField] private GameObject _buttonLayout;
        [Space][Header("SCENES TO LOAD")]
        [Space][SerializeField] private SceneIndices _optionsMenuScene;
        // [Space][SerializeField] private SceneIndices _loadMenuScene;
        [Space][SerializeField] private SceneIndices _mainMenuScene;
        [Space][Header("SCENE MODE")]
        [Space][SerializeField] private LoadSceneMode _loadSceneMode = LoadSceneMode.Additive;

        private void Awake()
        {
            Time.timeScale = 0f;
            GameManager.Instance.IsPauseMenuActive = true;
            GameManager.Instance.IsGamePaused = true;
            // DisableButtonDependingOnData();
        }

        private void Update()
        {
            _buttonLayout.SetActive(GameManager.Instance.IsPauseMenuActive);
            if(GameManager.Instance.IsPauseMenuActive && !GameManager.Instance.IsSelected)
            { 
                _resumeGameButton.Select();
                GameManager.Instance.IsSelected = true;
            }
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
        }

        // private void DisableButtonDependingOnData()
        // {
        //     // Check if there is a save file and enable the continue button if there is
        //     if (DataPersistenceManager.Instance.HasGameData()) return;
        //     _loadGameButton.interactable = false;
        // }

        #region Button Clicks
        
        public void OnResumeGameClicked()
        {
            GameManager.Instance.IsGamePaused = false;
            SceneLoader.Instance.UnloadSceneAsync();
        }
        
        public void OnOptionsMenuClicked()
        {
            SceneLoader.Instance.LoadSceneAsync(_optionsMenuScene, _loadSceneMode);
        }
        
        public void OnSaveGameClicked()
        {
            //DataPersistenceManager.Instance.ShowSaveAnimation = true;
            DataPersistenceManager.Instance.SaveGame();
        }
        
        // public void OnLoadMenuClicked()
        // {
        //     SceneLoader.Instance.LoadSceneAsync(_loadMenuScene, _loadSceneMode);
        // }
        
        public void OnMainMenuClicked()
        {
            GameManager.Instance.IsGamePaused = false;
            SceneLoader.Instance.LoadSceneAsync(_mainMenuScene);
        }
        
        public void OnQuitGameClicked()
        {
            Application.Quit();
        }
        
        #endregion
    }
}