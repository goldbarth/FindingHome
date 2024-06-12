using System;
using DataPersistence;
using UnityEngine;

namespace PuzzleHandler
{
    public class DoorOpenerBootTrigger : MonoBehaviour, IDataPersistence
    {
        public static event Action OnBootCollision;

        private bool _isDoorOpen;
        private bool _booted;
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player") && !col.isTrigger && !_booted && !_isDoorOpen)
            {
                OnBootCollision?.Invoke();
                _booted = true;
            }
        }

        public void LoadData(GameData data)
        {
            _isDoorOpen = data.isDoorOpen;
        }

        public void SaveData(GameData data)
        {
            data.isDoorOpen = _isDoorOpen;
        }
    }
}