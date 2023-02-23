using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace DataPersistence
{
    // This class is used to convert, save and load data from a file
    public class FileDataHandler
    {
        private readonly string _dataPath;
        private readonly string _dataFileName;

        // used for XOR encryption
        private const string ENCRYPTION_CODE_WORD = "Dreirad";
        private readonly bool _useEncryption = false;

        public FileDataHandler(string dataPath, string dataFileName, bool useEncryption)
        {
            _dataPath = dataPath;
            _dataFileName = dataFileName;
            _useEncryption = useEncryption;
        }

        #region Save/Load

         public void Save(GameData data, string profileId)
        {
            if (profileId == null)
                return;
            
            // use path.combine because different OS´s having different path separators
            var fullPath = Path.Combine(_dataPath, profileId, _dataFileName);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? string.Empty);
                
                var dataToStore = JsonUtility.ToJson(data, true);
                
                // encrypt the data if is selected in the inspector
                if (_useEncryption)
                    dataToStore = XorCipher(dataToStore);
                
                using var stream = new FileStream(fullPath, FileMode.Create);
                using var writer = new StreamWriter(stream);
                writer.Write(dataToStore);
            }
            catch (Exception e)
            {
                Debug.Log($"Error occured when trying to save data from file: {fullPath}\n{e}");
            }
        }
        
        public GameData Load(string profileId)
        {
            if (profileId == null)
                return null;
            
            // use path.combine because different OS´s having different path separators
            var fullPath = Path.Combine(_dataPath, profileId, _dataFileName);
            GameData loadedData = null;
            if (File.Exists(fullPath))
            {
                try
                {
                    // using filestream over file.writealltext to avoid locking the file and allow other
                    // processes to access it (e.g. real time data from a memory/network stream, multiple inputs)
                    using var stream = File.Open(fullPath, FileMode.Open);
                    using var reader = new StreamReader(stream);
                    var dataToLoad = reader.ReadToEnd(); // load the serialized data from the file
                    
                    // decrypt the data if is selected in the inspector
                    if (_useEncryption)
                        dataToLoad = XorCipher(dataToLoad);

                    // deserialize the data from json back into the C# object
                    loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Error occured when trying to load file at path: {fullPath}.\n{e}");
                }
            }

            return loadedData;
        }
        
        #endregion
        
        #region Extensions
        
        public void Delete(string profileId)
        {
            if (profileId == null)
                return;
            
            var fullPath = Path.Combine(_dataPath, profileId, _dataFileName);
            try
            {
                if (File.Exists(fullPath))
                    Directory.Delete(Path.GetDirectoryName(fullPath) ?? string.Empty, true);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Error occured when trying to delete data from file: {fullPath}\n{e}");
            }
        }

        public Dictionary<string, GameData> LoadAllProfiles()
        {
            var profileDictionary = new Dictionary<string, GameData>();

            // loop all directory names in the data directory path
            var directoryInfos = new DirectoryInfo(_dataPath).EnumerateDirectories();
            foreach (var directoryInfo in directoryInfos)
            {
                var profileId = directoryInfo.Name;
                
                // check if the data file exists. if it not, skip this profile
                var fullPath = Path.Combine(_dataPath, profileId, _dataFileName);
                if (!File.Exists(fullPath))
                {
                    Debug.Log($"Skipping directory, all files are loaded or there are no existing file.");
                    continue;
                }
                
                var profileData = Load(profileId);
                if (profileData != null)
                {
                    profileDictionary.Add(profileId, profileData);
                    Debug.Log($"Profile was loaded correctly. ProfileId: {profileId} at path {fullPath}.");
                }
                else
                    Debug.LogWarning($"Tried to load profile but something went wrong. " +
                                     $"ProfileId: {profileId} at path {fullPath}.");
            }

            return profileDictionary;
        }

        public string GetLatestProfileId()
        {
            string getLatestProfileId = null;
            
            var gameDataProfiles = LoadAllProfiles();
            foreach (var (profileId, gameData) in gameDataProfiles)
            {
                if (gameData == null)
                    continue;
                
                // if this is the first profile we´re checking, then set it as the most recent
                if (getLatestProfileId == null)
                {
                    getLatestProfileId = profileId;
                }
                else // otherwise, compare to see which date is the most recent
                {
                    var latestDateTime = DateTime.FromBinary(gameDataProfiles[getLatestProfileId].lastUpdated);
                    var newDateTime = DateTime.FromBinary(gameData.lastUpdated);
                    // the greatest DateTime value is the most recent
                    if (newDateTime > latestDateTime)
                        getLatestProfileId = profileId;
                }
            }
            
            return getLatestProfileId;
        }

        // XOR encryption with a code word 
        private static string XorCipher(string data)
        {
            var modifiedData = "";
            for (var i = 0; i < data.Length; i++)
                modifiedData += (char) (data[i] ^ ENCRYPTION_CODE_WORD[i % ENCRYPTION_CODE_WORD.Length]);
            
            return modifiedData;
        }
        
        #endregion
    }
}
