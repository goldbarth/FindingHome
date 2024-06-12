using DataPersistence;
using HelpersAndExtensions;
using ObstacleHandler;
using UnityEngine;

namespace Player.PlayerData
{
    public class DeathCount : MonoBehaviour, IDataPersistence
    {
        private int _deathCount;
        
        private void OnEnable()
        {
            RespawnCollision.OnTrapCollisionEvent += OnPlayerDeath;
        }
        
        private void OnDisable()
        {
            RespawnCollision.OnTrapCollisionEvent -= OnPlayerDeath;
        }
        
        private void OnPlayerDeath()
        {
            _deathCount++;
            Debug.Log("Death Count: " + _deathCount);
        }

        public void LoadData(GameData data)
        {
            _deathCount = data.deathCount;
        }

        public void SaveData(GameData data)
        {
            data.deathCount = _deathCount;
            Debug.Log("Death Count (data): " + data.deathCount);
        }
    }
}
