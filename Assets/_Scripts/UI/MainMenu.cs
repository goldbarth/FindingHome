using System;
using DataPersistence;
using SceneHandler;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

//TODO: menu audio in all menu scenes
namespace UI
{
    public class MainMenu : Menu
    {
        [Space][Header("MENU BUTTONS")]
        [Space][SerializeField] private Button newGameButton;
        [Space][SerializeField] private Button continueGameButton;
        [Space][SerializeField] private Button loadGameButton;
        [Space][Header("MENU CANVAS")]
        [Space][SerializeField] private GameObject menuCanvas;
        [Space][Header("PARALLAX BACKGROUND")]
        [Space][SerializeField] private GameObject background;

        private bool _buttonWasSelected = false;

        private void Awake()
        {
            GameManager.Instance.IsMenuActive = true;
            DataPersistenceManager.Instance.InitializeMenuAudioProfileId();
        }

        private void Start()
        {
            DisableButtonsDependingOnData();
        }

        private void Update()
        {
            menuCanvas.SetActive(GameManager.Instance.IsMenuActive);
            background.SetActive(GameManager.Instance.IsMenuActive);
            if (GameManager.Instance.IsMenuActive && !_buttonWasSelected)
            {
                newGameButton.Select();
                _buttonWasSelected = true;
            }
        }

        public void DisableButtonsDependingOnData()
        {
            if (DataPersistenceManager.Instance.HasGameData()) return;
            continueGameButton.interactable = false;
            loadGameButton.interactable = false;
        }

        #region Button Clicks

        public void OnNewGameClicked()
        {
            GameManager.Instance.IsNewGame = true;
            LoadSceneIsLoadMenu();
        }

        public void OnLoadGameClicked()
        {
            GameManager.Instance.IsNewGame = false;
            LoadSceneIsLoadMenu();
        }

        public void OnContinueGameClicked()
        {
            GameManager.Instance.IsGameStarted = true;
            DataPersistenceManager.Instance.ChangeSelectedProfileId(DataPersistenceManager.Instance.GetLatestProfileId());
            SceneLoader.Instance.LoadSceneAsync(SceneIndex.Level1, showProgress: true);
        }
        
        public void OnOptionsMenuClicked()
        {
            SceneLoader.Instance.LoadSceneAsync(SceneIndex.OptionsMenu, LoadSceneMode.Additive);
            GameManager.Instance.IsMenuActive = false;
            _buttonWasSelected = false;
        }
        
        public void OnExitClicked()
        {
            Application.Quit();
        }
        
        private void LoadSceneIsLoadMenu()
        {
            SceneLoader.Instance.LoadSceneAsync(SceneIndex.LoadMenu, LoadSceneMode.Additive);
            GameManager.Instance.IsMenuActive = false;
            _buttonWasSelected = false;
        }

        #endregion
    }
}