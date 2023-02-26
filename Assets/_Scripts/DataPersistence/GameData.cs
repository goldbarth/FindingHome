using DataPersistence.Serializable;
using UnityEngine;
using System;

namespace DataPersistence
{
    [Serializable]
    public class GameData
    {
        public Vector2 playerPosition;
        public int deathCount;
        public int level;
        public float masterVolume;
        public float sfxVolume;
        public float musicVolume;
        
        public long lastUpdated;
        
        // key = id of collectable, value = collected or not
        public SerializableDictionary<string, bool> collectables; 

        // default values in the constructor
        public GameData()
        {
            playerPosition = Vector3.zero;
            deathCount = 0;
            level = 1;
            
            masterVolume = 0.8f;
            sfxVolume = 0.8f;
            musicVolume = 0.8f;
            
            collectables = new SerializableDictionary<string, bool>();
        }

        // TODO: expand the range with (sub)goals for completion
        public int GetPercentageComplete()
        {
            var totalCollected = 0;
            foreach (var collected in collectables.Values)
                if (collected)
                    totalCollected++;

            // 0 can't be divided by 0, so it is set to -1
            var percentageCompleted = -1;
            if (collectables.Count != 0)
                percentageCompleted = totalCollected * 100 / collectables.Count;
            if (percentageCompleted == -1)
                percentageCompleted = 0;

            return percentageCompleted;
        }
    }
}
