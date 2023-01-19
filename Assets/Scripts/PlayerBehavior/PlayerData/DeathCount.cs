using System;
using DataPersistence;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerBehavior.PlayerData
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

        public void LoadData(ref GameData data)
        {
            _deathCount = data.deathCount;
        }

        public void SaveData(ref GameData data)
        {
            data.deathCount = _deathCount;
        }
    }
}
