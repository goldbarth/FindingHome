using System;
using DataPersistence;
using SceneHandler;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenu : Menu, IDataPersistence
    {
        [Space][Header("MENU BUTTONS")]
        [Space][SerializeField] private Button newGameButton;
        [Space][SerializeField] private Button continueGameButton;
        [Space][SerializeField] private Button loadGameButton;
        [Space][Header("MENU CANVAS")]
        [Space][SerializeField] private GameObject menuCanvas;
        [Space][Header("PARALLAX BACKGROUND")]
        [Space][SerializeField] private GameObject background;
        [Space][Header("AUDIO")]
        [Space][SerializeField] private AudioMixer audioMixer;
        [Space][Header("SCENES TO LOAD")]
        [Space][SerializeField] private SceneIndices continueCurrentGameScene;
        [Space][SerializeField] private SceneIndices optionsMenuScene;
        [Space][SerializeField] private SceneIndices loadMenuScene;
        [Space][Header("SCENE MODE")]
        [Space][SerializeField] private LoadSceneMode loadSceneMode = LoadSceneMode.Additive;

        
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
            SceneLoader.Instance.LoadSceneAsync(continueCurrentGameScene, showProgress: true);
        }
        
        public void OnOptionsMenuClicked()
        {
            SceneLoader.Instance.LoadSceneAsync(optionsMenuScene, loadSceneMode);
            GameManager.Instance.IsMenuActive = false;
            _buttonWasSelected = false;
        }
        
        public void OnExitClicked()
        {
            Application.Quit();
        }
        
        private void LoadSceneSaveSlotMenu()
        {
            SceneLoader.Instance.LoadSceneAsync(loadMenuScene, loadSceneMode);
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
                
                audioMixer.SetFloat("Master", data.masterVolume);
                audioMixer.SetFloat("SFX", data.masterVolume);
                audioMixer.SetFloat("Music", data.masterVolume);
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