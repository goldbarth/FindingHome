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
        private readonly string _menuAudioProfileId = "menu_audio";

        // used for XOR encryption
        private const string ENCRYPTION_CODE = "Grünes Dreirad ohne Stützräder, aber mit einem großen Korb und Luftballon, der hinten an den Streben befestigt ist.";
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
            
            // path.combine for different OS´s
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
                Debug.Log($"Error occured when trying to save ProfileId: {profileId} from file at path: {fullPath}\n{e}");
            }
            finally
            {
                Debug.Log($"ProfileId: {profileId} saved to file at path: {fullPath}");
            }
        }
         
         public GameData Load(string profileId)
        {
            if (profileId == null)
                return null;
            
            // path.combine for different OS´s
            var fullPath = Path.Combine(_dataPath, profileId, _dataFileName);
            GameData loadedData = null;
            if (File.Exists(fullPath))
            {
                try
                {
                    // using file.open over file.writealltext to avoid locking the file and
                    // allow other processes to access it.
                    using var stream = File.Open(fullPath, FileMode.Open);
                    using var reader = new StreamReader(stream);
                    var dataToLoad = reader.ReadToEnd(); // load the serialized data from the file

                    // decrypt the data if is selected in the inspector
                    if (_useEncryption)
                        dataToLoad = XorCipher(dataToLoad);

                    // deserialize the data
                    loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Error occured when trying to load ProfileId: {profileId} from file at path: {fullPath}.\n{e}");
                }
                finally
                {
                    Debug.Log($"ProfileId: {profileId} loaded from file at path: {fullPath}");
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
                Debug.LogWarning($"Error occured when trying to delete ProfileId: {profileId} from file at path: {fullPath}\n{e}");
            }
            finally
            {
                Debug.Log($"ProfileId: {profileId} deleted from file at path: {fullPath}.");
            }
        }

        public Dictionary<string, GameData> GetAllProfiles(bool getAllProfiles = false, bool skipGameplayData = false)
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
                    Debug.Log("Skipping directory, all files are loaded or there are no existing file.");
                    continue;
                }

                if (!getAllProfiles)
                {
                    // skip the gameplay data /bc menu audio should be loaded
                    if (skipGameplayData && profileId != _menuAudioProfileId)
                        continue;
                
                    // skip the menu audio profile /bc it should not be loaded to save slots
                    if (!skipGameplayData && profileId == _menuAudioProfileId) 
                        continue;
                }

                try
                {
                    var profileData = Load(profileId);
                    if (profileData != null)
                    {
                        profileDictionary.Add(profileId, profileData);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Tried to locate ProfileId: {profileId} at path: {fullPath} but the file is not there or is corrupted.\n{e}");
                }
                finally
                {
                    Debug.Log($"ProfileId: {profileId} was correct located at path: {fullPath}.");
                }
            }

            return profileDictionary;
        }

        public string GetLatestProfileId()
        {
            string getLatestProfileId = null;
            
            var gameDataProfiles = GetAllProfiles();
            foreach (var (profileId, gameData) in gameDataProfiles)
            {
                if (gameData == null)
                    continue;

                // if this is the first profile, set it as the latest
                if (getLatestProfileId == null)
                    getLatestProfileId = profileId;
                else // compare to see which date is the latest
                {
                    var latestDateTime = DateTime.FromBinary(gameDataProfiles[getLatestProfileId].lastUpdated);
                    var newDateTime = DateTime.FromBinary(gameData.lastUpdated);
                    // the greatest DateTime value is the latest
                    if (newDateTime > latestDateTime)
                        getLatestProfileId = profileId;
                }
            }
            
            return getLatestProfileId;
        }
        
        // XOR encryption. disclaimer: this is not a secure encryption method
        // and almost obsolete for a single player game
        private static string XorCipher(string data)
        {
            var modifiedData = string.Empty;
            for (var i = 0; i < data.Length; i++)
                modifiedData += (char) (data[i] ^ ENCRYPTION_CODE[i % ENCRYPTION_CODE.Length]);
            
            return modifiedData;
        }
        
        #endregion
    }
}
