using System;
using DataPersistence.Serializable;
using UnityEngine;

namespace DataPersistence
{
    [Serializable]
    public class GameData
    {
        public Vector2 playerPosition;
        public int deathCount;
        public int level;
        
        public long lastUpdated;
        
        public SerializableDictionary<string, bool> collectables; // key = id of collectable, value = collected or not

        // the values defined in this constructor will be the default values
        // the game starts with when there is no data to load
        public GameData()
        {
            playerPosition = Vector3.zero;
            deathCount = 0;
            level = 1;
            
            collectables = new SerializableDictionary<string, bool>();
        }

        // TODO: expand the range with (sub)goals for completion
        public int GetPercentageComplete()
        {
            var totalCollected = 0;
            foreach (var collected in collectables.Values)
            {
                if (collected) 
                    totalCollected++;
            }
            
            // ensure we don´t divide by 0 when calculating the percentage
            var percentageCompleted = -1;
            if (collectables.Count != 0)
                percentageCompleted = totalCollected * 100 / collectables.Count; // Dividing by zero is mathematically "undefined" and will result in an error
            

            return percentageCompleted;
        }
    }
}
