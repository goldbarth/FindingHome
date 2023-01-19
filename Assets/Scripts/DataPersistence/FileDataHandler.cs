using System;
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
        private readonly string _encrypionCodeWord = "Dreirad";
        
        public FileDataHandler(string dataPath, string dataFileName, bool useEncryption)
        {
            _dataPath = dataPath;
            _dataFileName = dataFileName;
            _useEncryption = useEncryption;
        }
        
        public GameData Load()
        {
            // use path.combine to account for different OS´s having different path separators
            var fullPath = Path.Combine(_dataPath, _dataFileName);
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
        
        public void Save(GameData data)
        {
            var fullPath = Path.Combine(_dataPath, _dataFileName);
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
        
        // XOR encryption with a code word to make it harder to read the data in the file (not secure) 
        private string EncryptDecrypt(string data)
        {
            var modifiedData = "";
            for (var i = 0; i < data.Length; i++)
                modifiedData += (char) (data[i] ^ _encrypionCodeWord[i % _encrypionCodeWord.Length]); 
            
            return modifiedData;
        }
    }
}
