using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DataPersistence
{
    // This class is used to convert, save and load data from a file.
    public class FileDataHandler
    {
        private readonly string _dataPath;
        private readonly string _dataFileName;
        private readonly string _backupExtension = ".bak";

        // used for XOR encryption
        private readonly bool _useEncryption = false;
        private const string ENCRYPTION_CODE_WORD = "Dreirad";

        public FileDataHandler(string dataPath, string dataFileName, bool useEncryption)
        {
            _dataPath = dataPath;
            _dataFileName = dataFileName;
            _useEncryption = useEncryption;
        }
        
        
        public GameData Load(string profileId, bool allowRestoreFromBackup = true)
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
                    
                    if (_useEncryption)
                        dataToLoad = EncryptDecrypt(dataToLoad);

                    // deserialize the data from json back into the C# object
                    loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
                }
                catch (Exception e)
                {
                    // prevent an infinite recursion loop if the backup file is corrupt
                    if (allowRestoreFromBackup)
                    {
                        Debug.LogWarning($"Error occured when trying to load data from file: {fullPath}." +
                                         $"Attempting to rollback to backup file.\n{e}");
                        var rollbackSuccess = AttemptRollback(fullPath);
                        if (rollbackSuccess)
                        {
                            // try to load again recursively
                            loadedData = Load(profileId, false);
                        }
                        else 
                        {
                            Debug.LogWarning($"Error occured when trying to load file at path: {fullPath} " +
                                           $"and backup did not work.\n{e}");
                        }
                    }
                }
            }

            return loadedData;
        }
        
        public void Save(GameData data, string profileId)
        {
            if (profileId == null)
                return;
            
            // use path.combine to account for different OS´s having different path separators
            var fullPath = Path.Combine(_dataPath, profileId, _dataFileName);
            var backupFilePath = fullPath + _backupExtension;                                                                                                                                                                                                       
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? string.Empty);
                
                var dataToStore = JsonUtility.ToJson(data, true); // convert to json
                
                // encrypt the data if needed
                if (_useEncryption)
                    dataToStore = EncryptDecrypt(dataToStore);
                
                // using filestream over file.writealltext to avoid locking the file and allow other
                // processes to access it (e.g. real time data from a memory/network stream, multiple inputs)
                using var stream = new FileStream(fullPath, FileMode.Create); // create file
                using var writer = new StreamWriter(stream);
                writer.Write(dataToStore); // write to file
                
                // verify that the file was written correctly
                var verifiedGameData = Load(profileId);
                // create a backup of the file
                if (verifiedGameData != null)
                {
                    File.Copy(fullPath, backupFilePath, true);
                }
                else
                {
                    throw new Exception("Save file could not be verified and backup could not be created.");
                    // if the file was not written correctly, delete it and restore the backup
                    // File.Delete(fullPath);
                    // File.Copy(backupFilePath, fullPath, true);
                }
            }
            catch (Exception e)
            {
                Debug.Log($"Error occured when trying to save data from file: {fullPath}\n{e}");
            }
        }
        
        public void Delete(string profileId)
        {
            if (profileId == null)
                return;
            
            // use path.combine to account for different OS´s having different path separators
            var fullPath = Path.Combine(_dataPath, profileId, _dataFileName);
            try
            {
                if (File.Exists(fullPath))
                {
                    Directory.Delete(Path.GetDirectoryName(fullPath) ?? string.Empty, true);
                }
                else
                {
                    Debug.Log($"File was not found at path: {fullPath}");
                }
            }
            catch (Exception e)
            {
                Debug.Log($"Error occured when trying to delete data from file: {fullPath}\n{e}");
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
                    Debug.LogWarning($"Skipping directory, all files are loaded or there are no existing file there.");
                    continue;
                }
                
                var profileData = Load(profileId);
                if (profileData != null)
                {
                    profileDictionary.Add(profileId, profileData);
                }
                else
                {
                     Debug.LogWarning($"Tried to load profile but something went wrong. " +
                                   $"ProfileId: {profileId} at path {fullPath}.");
                }
            }

            return profileDictionary;
        }

        public string GetMostRecentlyUpdatedProfileId()
        {
            string mostRecentProfileId = null;
            
            var profilesGameData = LoadAllProfiles();
            foreach (var pair in profilesGameData)
            {
                var profileId = pair.Key;
                var gameData = pair.Value;

                if (gameData == null)
                    continue;
                
                // if this is the first profile we´re checking, then set it as the most recent
                if (mostRecentProfileId == null)
                {
                    mostRecentProfileId = profileId;
                }
                else // otherwise, compare to see which date is the most recent
                {
                    var mostRecentDateTime = DateTime.FromBinary(profilesGameData[mostRecentProfileId].lastUpdated);
                    var newDateTime = DateTime.FromBinary(gameData.lastUpdated);
                    // the greatest DateTime value is the most recent
                    if (newDateTime > mostRecentDateTime)
                        mostRecentProfileId = profileId;
                }
            }
            
            return mostRecentProfileId;
        }

        // XOR encryption with a code word 
        private static string EncryptDecrypt(string data)
        {
            var modifiedData = "";
            for (var i = 0; i < data.Length; i++)
                modifiedData += (char) (data[i] ^ ENCRYPTION_CODE_WORD[i % ENCRYPTION_CODE_WORD.Length]);
            
            return modifiedData;
        }
        
        private bool AttemptRollback(string fullPath)
        {
            var success = false;
            var backupFilePath = fullPath + _backupExtension;
            try
            {
                if (File.Exists(backupFilePath))
                { 
                    File.Copy(backupFilePath, fullPath, true);
                    success = true;
                    Debug.Log($"Rollback successful. Restored backup file: {backupFilePath}");
                }
                else
                {
                    throw new Exception($"Rollback failed. Backup file not found: {backupFilePath}");
                }
            }
            catch (Exception e)
            {
                Debug.Log($"Error occured when trying to rollback data from file: {fullPath}\n{e}");
            }
            
            return success;
            // var backupFilePath = fullPath + _backupExtension;
            // try
            // {
            //     if (File.Exists(backupFilePath))
            //     {
            //         File.Copy(backupFilePath, fullPath, true);
            //         Debug.Log($"Rollback successful. Restored backup file: {backupFilePath}");
            //         return true;
            //     }
            //     
            //     Debug.LogWarning($"Rollback failed. Backup file not found: {backupFilePath}");
            //     return false;
            // }
            // catch (Exception e)
            // {
            //     Debug.Log($"Error occured when trying to rollback data from file: {fullPath}\n{e}");
            // }
// 
            // return false;
        }
    }
}
