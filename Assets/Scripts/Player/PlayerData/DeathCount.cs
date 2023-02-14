using UnityEngine.InputSystem;
using DataPersistence;
using UnityEngine;

namespace Player.PlayerData
{
    public class DeathCount : MonoBehaviour, IDataPersistence
    {
        private int _deathCount = 0;
        
        private void OnPlayerDeath()
        {
            _deathCount++;
        }

        private void Update()
        {
            // testing purpose
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
                _deathCount++;
        }

        public void LoadData(GameData data)
        {
            _deathCount = data.deathCount;
        }

        public void SaveData(GameData data)
        {
            data.deathCount = _deathCount;
        }
    }
}
