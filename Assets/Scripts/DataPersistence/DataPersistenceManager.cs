using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
 
namespace DataPersistence
{
    // This class is used to store the data of the game.
    // if the game is closed manually or crashed, the data will be saved in the file.
    public class DataPersistenceManager : MonoBehaviour
    {
        [Header("Debugging")]
        [SerializeField] private bool initializeDataIfNull = false; 
        
        [Header("File Storage Config")]
        [SerializeField] private string fileName;
        [SerializeField] private bool useEncryption;

        private string _selectedProfileId = "Slot";
        
        private GameData _gameData;
        private FileDataHandler _dataHandler;
        private List<IDataPersistence> _dataPersistenceObjects; // List of all objects that need to be saved and loaded

        public static DataPersistenceManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            
            _dataHandler = new FileDataHandler(Application.persistentDataPath ,fileName, useEncryption);
        }

        private void OnEnable()
        {
            // Subscribe to the events
            SceneManager.sceneLoaded += OnSceneLoaded; 
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }
        
        private void OnDisable()
        {
            // Unsubscribe to the events
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        // called after OnEnable and awake but before Start
        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _dataPersistenceObjects = FindAllDataPersistenceObjects();
            LoadGame();
            Debug.Log("OnSceneLoaded: " + scene.name);
        }
        
        public void OnSceneUnloaded(Scene scene)
        {
            SaveGame();
            Debug.Log("OnSceneUnloaded: " + scene.name);
        }
        
        public void NewGame()
        {
            _gameData = new GameData();
        }
        
        public void LoadGame()
        {
            _gameData = _dataHandler.Load(_selectedProfileId);

            if (_gameData == null && initializeDataIfNull)
            {
                NewGame();
            }
            else
            {
                Debug.Log("Game data is null");
                return;
            }
            
            // If there is no game data saved, warning message will be printed
            if (_gameData == null)
            {
                Debug.Log("No save data found. A new Game needs to be started before date can be loaded.");
                return;
            }
            
            // push data to other scripts that need it (e.g. player stats, inventory, etc.)
            foreach (var dataPersistenceObject in _dataPersistenceObjects)
            {
                dataPersistenceObject.LoadData(ref _gameData);
            }
        }
        
        public void SaveGame()
        {
            // If there is no game data saved, warning message will be printed
            if (_gameData == null)
            {
                Debug.Log("No save data found. A new Game needs to be started before date can be saved.");
                return;
            }
            
            // Save all data from other scripts so they can update the GameData
            foreach (var dataPersistenceObject in _dataPersistenceObjects)
            {
                dataPersistenceObject.SaveData(ref _gameData);
            }
            
            _dataHandler.Save(_gameData, _selectedProfileId);
        }
        
        private List<IDataPersistence> FindAllDataPersistenceObjects()
        {
            // Find all objects that implement IDataPersistence
            var dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>(); 

            return new List<IDataPersistence>(dataPersistenceObjects);
        }

        public bool HasGameData()
        {
            return _gameData != null;
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }

        public Dictionary<string, GameData> GetAllProfilesGameData()
        {
            return _dataHandler.LoadAllProfiles();
        }
    }
}
