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

        private readonly SceneIndex[] _menuScene = {SceneIndex.Init, SceneIndex.LoadMenu, SceneIndex.OptionsMenu, SceneIndex.PauseMenu};

        private string _selectedProfileId;
        private readonly string _menuAudioProfileId = "menu_audio";
        
        public bool DisableDataPersistence => disableDataPersistence;
        public bool UseEncryption => useEncryption;
        public string MenuAudioProfileId => _menuAudioProfileId;
        
        
        #region Events
        
        protected override void Awake()
        {
            base.Awake();
            
            if (disableDataPersistence) 
                Debug.LogWarning("Data Persistence is DISABLED!");
            
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
            
            foreach (var index in _menuScene)
            {
                if (scene.buildIndex == (int)index)
                    Debug.Log("Loaded a menu scene. No persistent data needed to load.");
                else if (scene.buildIndex != (int)index)
                {
                    _dataPersistenceObjects = FindAllDataPersistenceObjects();
                    LoadGame();
                }
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
            _dataHandler.Save(_gameData, _selectedProfileId);
        }
        
        public void LoadGame()
        {
            if (disableDataPersistence) return;
            
            GameManager.Instance.OnRoomReset = false;
            _gameData = _dataHandler.Load(_selectedProfileId);
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
            var profiles = GetAllProfilesGameData(getAllProfiles: true); 
            // if audio profile is the only profile from all profiles, return false
            // (to prevent enabling the continue button and loading the audio profile on continue)
            if (profiles.Count == 1 && profiles.ContainsKey(_menuAudioProfileId))
                return false;

            return _gameData != null;
        }
        
        public void InitializeMenuAudioProfileId()
        {
            try
            {
                var containsMenuAudioProfile = GetAllProfilesGameData(getAllProfiles: true).ContainsKey(_menuAudioProfileId);
                if (!containsMenuAudioProfile)
                {
                    _dataHandler.Save(new GameData(), _menuAudioProfileId);
                }

                _gameData = _dataHandler.Load(_menuAudioProfileId);
            }
            catch (Exception e)
            {
                Debug.LogError($"Audio profile couldn´t be loaded on Application-Start\n{e}");
            }
            finally
            {
                Debug.Log("Audio profile was loaded on Application-Start");
            }
        }
        
        private void InitializeProfileId()
        {
            // if the profile id is null, get the latest profile id
            _selectedProfileId ??= _dataHandler.GetLatestProfileId();
        }
        
        public string GetLatestProfileId()
        {
            return _selectedProfileId = _dataHandler.GetLatestProfileId();
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
