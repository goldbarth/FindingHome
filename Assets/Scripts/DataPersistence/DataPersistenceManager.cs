using System.Collections;
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
        [Header("DEBUGGING")] [Tooltip("If checked, the data will NOT be saved!!!")]
        [SerializeField] private bool disableDataPersistence = false;
        [Space][SerializeField] private bool initializeDataIfNull = false;
        [Space][SerializeField] private bool overwriteSelectedProfileId = false;
        [Space][SerializeField] private string testSelectedProfileId = "test file";
        
        [Header("FILE STORAGE CONFIG")]
        [SerializeField] private string fileName;
        [Space][SerializeField] private bool useEncryption;
        
        [Header("AUTO SAVING CONFIG")]
        [SerializeField] private float autoSaveInterval = 60f;

        private string _selectedProfileId;
        
        private GameData _gameData;
        private FileDataHandler _dataHandler;
        private List<IDataPersistence> _dataPersistenceObjects; // List of all objects that need to be saved and loaded
        private Coroutine _autoSaveCoroutine;

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

            if (disableDataPersistence) 
                Debug.LogWarning("Data Persistence is disabled!");
            
            _dataHandler = new FileDataHandler(Application.persistentDataPath ,fileName, useEncryption);
            InitializeSelectedProfileId();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // called after OnEnable and awake but before Start
        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _dataPersistenceObjects = FindAllDataPersistenceObjects();
            LoadGame();
            
            Debug.Log($"OnSceneLoaded: {scene.name}");
            
            // Start the auto save coroutine
            if (_autoSaveCoroutine != null)
                StopCoroutine(_autoSaveCoroutine);
    
            _autoSaveCoroutine = StartCoroutine(AutoSave());
        }

        public void ChangeSelectedProfileId(string newProfileId)
        {
            // update the profile id to use for saving and loading
            _selectedProfileId = newProfileId;
            // load the game, which will use that profile
            LoadGame();
        }
        public void DeleteProfileData(string profileId)
        {
            // delete the data for this profile id
            _dataHandler.Delete(profileId);
            // initialize the selected profile id
            InitializeSelectedProfileId();
            // reload the game, the newly selected profile id data matches
            LoadGame();
        }

        private void InitializeSelectedProfileId()
        {
            _selectedProfileId = _dataHandler.GetMostRecentlyUpdatedProfileId();
            if (overwriteSelectedProfileId)
            {
                _selectedProfileId = testSelectedProfileId;
                Debug.LogWarning($"Overwriting selected profile id with: {testSelectedProfileId}");
            }
        }

        public void NewGame()
        {
            _gameData = new GameData();
        }
        
        public void LoadGame()
        {
            if (disableDataPersistence) return;
            
            _gameData = _dataHandler.Load(_selectedProfileId);

            if (_gameData == null && initializeDataIfNull)
            {
                NewGame();
            }

            // push data to other scripts that need it (e.g. player pos, inventory, etc.)
            foreach (var dataPersistenceObject in _dataPersistenceObjects)
            {
                dataPersistenceObject.LoadData(_gameData);
            }
        }
        
        public void SaveGame()
        {
            if (disableDataPersistence) return;

            // If there is no game data saved, warning message will be printed
            if (_gameData == null)
            {
                Debug.Log("No save data found. A new Game needs to be started before data can be saved.");
                return;
            }
            
            // Save all data from other scripts so they can update the GameData
            foreach (var dataPersistenceObject in _dataPersistenceObjects)
            {
                dataPersistenceObject.SaveData(_gameData);
            }
            
            // timestamp the data it shows when it was last saved
            _gameData.lastUpdated = System.DateTime.Now.ToBinary();
            
            _dataHandler.Save(_gameData, _selectedProfileId);
        }
        
        private static List<IDataPersistence> FindAllDataPersistenceObjects()
        {
            // find all objects that implement IDataPersistence. if you want to save data from a new script, add the interface to it  
            // optionally, if in FindObjectsOfType, you can add true as a parameter, which will include inactive objects
            var dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<IDataPersistence>(); 

            return new List<IDataPersistence>(dataPersistenceObjects);
        }
        
        public Dictionary<string, GameData> GetAllProfilesGameData()
        {
            return _dataHandler.LoadAllProfiles();
        }
        
        private IEnumerator AutoSave()
        {
            while (true)
            {
                yield return new WaitForSeconds(autoSaveInterval);
                SaveGame();
                Debug.Log("Auto saving...");
            }
        }

        public bool HasGameData()
        {
            return _gameData != null;
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }
    }
}
