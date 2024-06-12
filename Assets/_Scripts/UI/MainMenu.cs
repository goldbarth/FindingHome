using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using DataPersistence;
using UnityEngine.UI;
using SceneHandler;
using UnityEngine;
using System;

namespace UI
{
    public class MainMenu : Menu, IDataPersistence
    {
        [Space][Header("MENU BUTTONS")]
        [Space][SerializeField] private Button _newGameButton;
        [Space][SerializeField] private Button _continueGameButton;
        [Space][SerializeField] private Button _loadGameButton;
        [Space][Header("MENU CANVAS")]
        [Space][SerializeField] private GameObject _menuCanvas;
        [Space][Header("PARALLAX BACKGROUND")]
        [Space][SerializeField] private GameObject _background;
        [Space][Header("AUDIO")]
        [Space][SerializeField] private AudioMixer _audioMixer;
        [Space][Header("SCENES TO LOAD")]
        [Space][SerializeField] private SceneIndices _continueCurrentGameScene;
        [Space][SerializeField] private SceneIndices _optionsMenuScene;
        [Space][SerializeField] private SceneIndices _loadMenuScene;
        [Space][SerializeField] private SceneIndices _creditsScreenScene;
        [Space][Header("SCENE MODE")]
        [Space][SerializeField] private LoadSceneMode _loadSceneMode = LoadSceneMode.Additive;

        
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
            _menuCanvas.SetActive(GameManager.Instance.IsMenuActive);
            _background.SetActive(GameManager.Instance.IsMenuActive);
            if (GameManager.Instance.IsMenuActive && !_buttonWasSelected)
            {
                _newGameButton.Select();
                _buttonWasSelected = true;
            }
        }

        public void DisableButtonsDependingOnData()
        {
            if (DataPersistenceManager.Instance.HasGameData()) return;
            _continueGameButton.interactable = false;
            _loadGameButton.interactable = false;
        }

        #region Button Clicks

        public void OnNewGameClicked()
        {
            GameManager.Instance.IsNewGame = true;
            LoadSceneSaveSlotMenu();
        }

        public void OnLoadGameClicked()
        {
            GameManager.Instance.IsNewGame = false;
            LoadSceneSaveSlotMenu();
        }

        public void OnContinueGameClicked()
        {
            GameManager.Instance.IsGameStarted = true;
            DataPersistenceManager.Instance.ChangeSelectedProfileId(DataPersistenceManager.Instance.GetLatestProfileId());
            DataPersistenceManager.Instance.SaveGame();
            SceneLoader.Instance.LoadSceneAsync(_continueCurrentGameScene, showProgress: true);
        }
        
        public void OnOptionsMenuClicked()
        {
            SceneLoader.Instance.LoadSceneAsync(_optionsMenuScene, _loadSceneMode);
            GameManager.Instance.IsMenuActive = false;
            _buttonWasSelected = false;
        }

        public void OnCreditsClicked()
        {
            SceneLoader.Instance.LoadSceneAsync(_creditsScreenScene, _loadSceneMode);
            GameManager.Instance.IsMenuActive = false;
            _buttonWasSelected = false;
        }

        public void OnExitClicked()
        {
            Application.Quit();
        }
        
        private void LoadSceneSaveSlotMenu()
        {
            SceneLoader.Instance.LoadSceneAsync(_loadMenuScene, _loadSceneMode);
            GameManager.Instance.IsMenuActive = false;
            _buttonWasSelected = false;
        }

        #endregion

        // set the stored audio from "menu_audio" profile to the audio mixer at app-start
        public void LoadData(GameData data)
        {
            try // fancy try catch block
            {
                if (data == null)
                    throw new ArgumentNullException(paramName: nameof(data), message: "Load-Data can't be null.");
                
                _audioMixer.SetFloat("Master", data.masterVolume);
                _audioMixer.SetFloat("SFX", data.masterVolume);
                _audioMixer.SetFloat("Music", data.masterVolume);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"{e}\nAudio data couldn't be loaded. There is no save file to to read.");
            }
            finally
            {
                Debug.Log("Audio data loaded in Main-Menu.");
            }
        }
        
        public void SaveData(GameData data)
        { }
    }
}