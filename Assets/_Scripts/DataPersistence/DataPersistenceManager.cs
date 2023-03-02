using UnityEngine.SceneManagement;
using System.Collections.Generic;
using SceneHandler;
using System.Linq;
using UnityEngine;
using System;
using AddIns;

namespace DataPersistence
{
    // This class is used to store the data of the game.
    public class DataPersistenceManager : Singleton<DataPersistenceManager>
    {
        [Header("DEBUGGING")] [Tooltip("If checked, the data will NOT be saved!!!")]
        [SerializeField] private bool disableDataPersistence = false;
        [Space][SerializeField] private bool initializeDataIfNull = false;

        [Header("FILE STORAGE CONFIG")]
        [SerializeField] private string fileName;
        [Space][SerializeField] private bool useEncryption;

        private GameData _gameData;
        private FileDataHandler _dataHandler;
        private List<IDataPersistence> _dataPersistenceObjects; // list of all objects that need to be saved and loaded

        private readonly List<SceneIndices> _menuScenes = new(){SceneIndices.Init, SceneIndices.LoadMenu, SceneIndices.MainMenu, SceneIndices.OptionsMenu};

        private string _selectedProfileId;
        private readonly string _menuAudioProfileId = "menu_audio";

        public bool DisableDataPersistence => disableDataPersistence;

        #region Events
        
        protected override void Awake()
        {
            base.Awake();
            
            if (disableDataPersistence) Debug.LogWarning("Data Persistence is DISABLED!");
            _dataHandler = new FileDataHandler(Application.persistentDataPath ,fileName, useEncryption);
            InitializeProfileId();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // called after OnEnable and Awake but before Start
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"Scene Loaded: {scene.name}");

            // if the application has started, the menu audio profile is always selected
            if (GameManager.Instance.OnApplicationStart)
                _selectedProfileId = _menuAudioProfileId;
            // if the current scene is in menuScenes, the menu audio profile is selected
            else if (_menuScenes.Contains((SceneIndices)scene.buildIndex))
            {
                _dataPersistenceObjects = FindAllDataPersistenceObjects();
                Debug.Log($"Loaded a menu scene: {_menuScenes.IndexOf((SceneIndices)scene.buildIndex)}. No persistent data needed to load.");
            }
            else
            {
                _dataPersistenceObjects = FindAllDataPersistenceObjects();
                LoadGame();
            }

            GameManager.Instance.OnApplicationStart = false;
        }

        // TODO: maybe it has use later
        // if the game is closed manually or crashed, the data will be saved in the file.
        //private void OnApplicationQuit()
        //{
        //    SaveGame();
        //}
        
        #endregion

        #region Save/Load Game

        public void SaveGame()
        {
            if (disableDataPersistence) return;
            if (_gameData == null)
            {
                Debug.Log("No save data found. A new Game needs to be started before data can be saved.");
                return;
            }
            
            // save all data from other scripts so they can update the GameData
            foreach (var dataPersistenceObject in _dataPersistenceObjects)
                dataPersistenceObject.SaveData(_gameData);

            // timestamp the data it shows when it was last saved
            _gameData.lastUpdated = DateTime.Now.ToBinary();
            _dataHandler.SaveData(_gameData, _selectedProfileId);
        }

        public void LoadGame()
        {
            if (disableDataPersistence) return;
            
            GameManager.Instance.OnRoomReset = false; // prevents to save the players position. e.g. when falling into a pit
            _gameData = _dataHandler.LoadData(_selectedProfileId);
            if (_gameData == null && initializeDataIfNull)
                NewGame();

            // push data to other scripts that need it (e.g. player pos, inventory, etc.)
            foreach (var dataPersistenceObject in _dataPersistenceObjects)
                dataPersistenceObject.LoadData(_gameData);
        }

        #endregion
        
        #region Extentions

        public void NewGame()
        {
            _gameData = new GameData();
        }
        
        public bool HasGameData()
        {
            // if audio profile is the only profile from all profiles, return false
            // (to prevent enabling the continue button and loading the audio profile on continue)
            if (GetProfileCount() == 1 && ContainsAudioProfile())
            {
                LoadAudioProfile();
                return false;
            }

            return _gameData != null;
        }
        
        public void InitializeMenuAudioProfileId()
        {
            try
            {
                if (!ContainsAudioProfile())
                    CreateNewAudioProfile();
            }
            catch (Exception e)
            {
                Debug.LogError($"Audio profile couldn´t be loaded on Application-Start\n{e.Message}");
            }
            finally
            {
                _gameData = LoadAudioProfile();
                Debug.Log("Audio profile was loaded on Application-Start");
            }
        }
        
        private void InitializeProfileId()
        {
            // if the profile id is null, get the latest profile id
            _selectedProfileId ??= _dataHandler.GetLatestProfileId();
        }
        
        private GameData LoadAudioProfile()
        {
            return _gameData = _dataHandler.LoadData(_menuAudioProfileId);
        }
        
        public void SaveAudioProfile()
        {
            // save all data from other scripts so they can update the GameData
            foreach (var dataPersistenceObject in _dataPersistenceObjects)
                dataPersistenceObject.SaveData(_gameData);

            // timestamp the data it shows when it was last saved
            _gameData.lastUpdated = DateTime.Now.ToBinary();
            _dataHandler.SaveData(_gameData, _menuAudioProfileId);
        }
        
        private void CreateNewAudioProfile()
        {
            _dataHandler.SaveData(new GameData(), _menuAudioProfileId);
        }
        
        public string GetLatestProfileId()
        {
            return _selectedProfileId = _dataHandler.GetLatestProfileId();
        }

        private int GetProfileCount()
        {
            return _dataHandler.GetAllProfiles(getAllProfiles: true).Count;
        }

        private bool ContainsAudioProfile()
        {
            return _dataHandler.ContainsAudioProfile();
        }
        
        public void ChangeSelectedProfileId(string newProfileId)
        {
            // update the profile id to use for saving and loading
            _selectedProfileId = newProfileId;
            LoadGame();
        }
        
        public void DeleteProfileData(string profileId)
        {
            _dataHandler.Delete(profileId);
            InitializeProfileId();
            LoadGame();
        }

        public Dictionary<string, GameData> GetAllProfilesGameData(bool getAllProfiles = false, bool skipGameplayData = false)
        {
            return _dataHandler.GetAllProfiles(getAllProfiles, skipGameplayData);
        }
        
        private static List<IDataPersistence> FindAllDataPersistenceObjects()
        {
            // enable true in FindObjectsOfType to include inactive objects
            var dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<IDataPersistence>();
            return new List<IDataPersistence>(dataPersistenceObjects);
        }
        
        #endregion
    }
}
