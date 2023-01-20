using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DataPersistence
{
    // This class is used to convert, save and load data from a file
    public class FileDataHandler
    {
        private string _dataPath;
        private string _dataFileName;
        
        // used for XOR encryption
        private bool _useEncryption = false;
        private const string ENCRYPTION_CODE_WORD = "Dreirad";

        public FileDataHandler(string dataPath, string dataFileName, bool useEncryption)
        {
            _dataPath = dataPath;
            _dataFileName = dataFileName;
            _useEncryption = useEncryption;
        }
        
        public GameData Load(string profileId)
        {
            // use path.combine to account for different OS´s having different path separators
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
                    
                    if (_useEncryption)
                        dataToLoad = EncryptDecrypt(dataToLoad);

                    // deserialize the data from json back into the C# object
                    loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
                }
                catch (Exception e)
                {
                    Debug.Log($"error occured when trying to load data from file: {fullPath}\n{e}");
                }
            }

            return loadedData;
        }
        
        public void Save(GameData data, string profileId)
        {
            var fullPath = Path.Combine(_dataPath, profileId, _dataFileName);
            try
            {
                // use path.combine to account for different OS´s having different path separators
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? string.Empty);
                
                var dataToStore = JsonUtility.ToJson(data , true); // convert to json
                
                // encrypt the data if needed
                if (_useEncryption)
                    dataToStore = EncryptDecrypt(dataToStore);
                
                // using filestream over file.writealltext to avoid locking the file and allow other
                // processes to access it (e.g. real time data from a memory/network stream, multiple inputs)
                using var stream = new FileStream(fullPath, FileMode.Create); // create file
                using var writer = new StreamWriter(stream);
                writer.Write(dataToStore); // write to file
            }
            catch (Exception e)
            {
                Debug.Log($"error occured when trying to save data from file: {fullPath}\n{e}");
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
                
                // check if the data file exists. if it doesn´t, then this folder isn´t a profile and should be skipped
                var fullPath = Path.Combine(_dataPath, profileId, _dataFileName);
                if (!File.Exists(fullPath))
                {
                    Debug.LogWarning($"Skipping directory when loading all profiles because it does not contain data: {profileId}.");
                }
                
                // load the game data for this profile and put it in the directory
                var profileData = Load(profileId);
                // ensure the profile data isn´t null, because if it is then something went wrong and we should let ourselves know
                if (profileData != null)
                {
                    profileDictionary.Add(profileId, profileData);
                }
                else
                {
                    Debug.LogError($"Tried to load profile but something went wrong. ProfileId: {profileId}.");
                }
            }

            return profileDictionary;
        }

        // XOR encryption with a code word to make it harder to read the data in the file (not secure) 
        private static string EncryptDecrypt(string data)
        {
            var modifiedData = "";
            for (var i = 0; i < data.Length; i++)
                modifiedData += (char) (data[i] ^ ENCRYPTION_CODE_WORD[i % ENCRYPTION_CODE_WORD.Length]); 
            
            return modifiedData;
        }
    }
}
