using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace DataPersistence
{
     public class NewFileHandler
    {
        private readonly string _dataPath;
        private readonly string _dataFileName;
        
        public NewFileHandler(string dataPath, string dataFileName)
        {
            _dataPath = dataPath;
            _dataFileName = dataFileName;
        }

        public void Save(GameData data, string profileId)
        {
            if (profileId == null)
                return;
            
            // use path.combine because different OS´s having different path separators
            var fullPath = Path.Combine(_dataPath, profileId, _dataFileName);
            // To serialize the hashtable and its key/value pairs,
            // you must first open a stream for writing.
            var stream = new FileStream(fullPath, FileMode.Create);
            // Construct a BinaryFormatter and use it to serialize the data to the stream.
            var formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(stream, data);
            }
            catch (SerializationException e)
            {
                Debug.Log("Failed to serialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                stream.Close();
            }
        }
        
        public GameData Load(string profileId)
        {
            if (profileId == null)
                return null;
            
            // use path.combine because different OS´s having different path separators
            var fullPath = Path.Combine(_dataPath, profileId, _dataFileName);
            GameData loadedData = null;
            // Open the file containing the data that you want to deserialize.
            var stream = new FileStream(fullPath, FileMode.Open);
            if (File.Exists(fullPath))
            {
                try
                {
                    var formatter = new BinaryFormatter();
                    
                    // Deserialize the gamedata from the file and
                    // assign the reference to the local variable.
                    loadedData = (GameData)formatter.Deserialize(stream);
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                    throw;
                }
                finally
                {
                    stream.Close();
                }
            }

            return loadedData;
        }
        
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

        public string GetLatestUpdatedProfileId()
        {
            string latestUpdatedProfileId = null;
            
            var gameDataProfiles = LoadAllProfiles();
            foreach (var (profileId, gameData) in gameDataProfiles)
            {
                if (gameData == null)
                    continue;
                
                // if this is the first profile we´re checking, then set it as the most recent
                if (latestUpdatedProfileId == null)
                {
                    latestUpdatedProfileId = profileId;
                }
                else // otherwise, compare to see which date is the most recent
                {
                    var latestDateTime = DateTime.FromBinary(gameDataProfiles[latestUpdatedProfileId].lastUpdated);
                    var newDateTime = DateTime.FromBinary(gameData.lastUpdated);
                    // the greatest DateTime value is the most recent
                    if (newDateTime > latestDateTime)
                        latestUpdatedProfileId = profileId;
                }
            }
            
            return latestUpdatedProfileId;
        }
    }
}