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
        
        public SerializableDictionary<string, bool> collectables; // key = id of collectable, value = collected or not

        // the values defined in this constructor will be the default values
        // the game starts with when there is no data to load
        public GameData()
        {
            playerPosition = Vector3.zero;
            deathCount = 0;
            level = 0;
            
            collectables = new SerializableDictionary<string, bool>();
        }
    }
}
