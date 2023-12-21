using DataPersistence.Serializable;
using UnityEngine;
using System;

namespace DataPersistence
{
    [Serializable]
    public class GameData
    {
        public Vector2 playerPosition = Vector3.zero;
        public float masterVolume = 0.8f;
        public float sfxVolume = 0.8f;
        public float musicVolume = 0.8f;
        public int eatableCount = 0;
        public int deathCount = 0;
        public int level = 1;
        
        public long lastUpdated;
        
        // key = id of collectable, value = collected or not
        public SerializableDictionary<string, bool> collectables = new();
        public SerializableDictionary<string, bool> eatables = new();

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
        
        public bool HasEatable()
        {
            foreach (var collected in eatables.Values)
                if (collected) return true;
            return false;
        }
    }
}
