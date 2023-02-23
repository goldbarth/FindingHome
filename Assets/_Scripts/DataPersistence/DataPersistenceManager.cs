using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using AddIns;
using SceneHandler;
using UnityEngine;
 
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

        private readonly SceneIndex[] _menuScene = {SceneIndex.MainMenu, SceneIndex.LoadMenu, SceneIndex.OptionsMenu, SceneIndex.PauseMenu};

        private string _selectedProfileId;
        
        private GameData _gameData;
        private FileDataHandler _dataHandler;
        private List<IDataPersistence> _dataPersistenceObjects; // List of all objects that need to be saved and loaded

        public bool DisableDataPersistence => disableDataPersistence;
        
        
        #region Events
        
        protected override void Awake()
        {
            base.Awake();
            
            if (disableDataPersistence) 
                Debug.LogWarning("Data Persistence is disabled!");
            
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
            Debug.Log($"OnSceneLoaded: {scene.name}");

            foreach (var index in _menuScene)
            {
                if (scene.buildIndex == (int)index)
                {
                    Debug.Log("Loaded a Menu scene. No persistent data needed to load.");
                }
                else if (scene.buildIndex != (int)index)
                {
                    _dataPersistenceObjects = FindAllDataPersistenceObjects();
                    LoadGame();
                }
            }
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
            
            // Save all data from other scripts so they can update the GameData
            foreach (var dataPersistenceObject in _dataPersistenceObjects)
                dataPersistenceObject.SaveData(_gameData);

            // timestamp the data it shows when it was last saved
            _gameData.lastUpdated = System.DateTime.Now.ToBinary();
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
            return _gameData != null;
        }
        
        private void InitializeProfileId()
        {
            _selectedProfileId = _dataHandler.GetLatestProfileId();
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

        public Dictionary<string, GameData> GetAllProfilesGameData()
        {
            return _dataHandler.LoadAllProfiles();
        }
        
        private static List<IDataPersistence> FindAllDataPersistenceObjects()
        {
            // enable true in FindObjectsOfType for include inactive objects
            var dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<IDataPersistence>();
            return new List<IDataPersistence>(dataPersistenceObjects);
        }
        
        #endregion
    }
}
