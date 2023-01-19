using System.Collections.Generic;
using System.Linq;
using UnityEngine;
 
namespace DataPersistence
{
    // This class is used to store the data of the game.
    // if the game is closed manually or crashed, the data will be saved in the file.
    public class DataPersistenceManager : MonoBehaviour
    {
        [Header("File Storage Config")]
        [SerializeField] private string fileName;
        [SerializeField] private bool useEncryption;
        
        private GameData _gameData;
        private FileDataHandler _dataHandler;
        private List<IDataPersistence> _dataPersistenceObjects; // List of all objects that need to be saved and loaded

        private void Start()
        {
            _dataHandler = new FileDataHandler(Application.persistentDataPath ,fileName, useEncryption);
            _dataPersistenceObjects = FindAllDataPersistenceObjects();
            LoadGame();
        }

        private List<IDataPersistence> FindAllDataPersistenceObjects()
        {
            // Find all objects that implement IDataPersistence
            var dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>(); 

            return new List<IDataPersistence>(dataPersistenceObjects);
        }

        public void NewGame()
        {
            _gameData = new GameData();
        }
        
        public void LoadGame()
        {
            _gameData = _dataHandler.Load();
            
            // If there is no game data, create a new one
            if (_gameData == null)
            {
                Debug.Log("No save data found. Initializing data to defaults.");
                NewGame();
            }
            
            // push data to other scripts that need it (e.g. player stats, inventory, etc.)
            foreach (var dataPersistenceObject in _dataPersistenceObjects)
            {
                dataPersistenceObject.LoadData(ref _gameData);
            }
        }
        
        public void SaveGame()
        {
            // Save all data from other scripts so they can update the GameData
            foreach (var dataPersistenceObject in _dataPersistenceObjects)
            {
                dataPersistenceObject.SaveData(ref _gameData);
            }
            
            _dataHandler.Save(_gameData);
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }
    }
}
