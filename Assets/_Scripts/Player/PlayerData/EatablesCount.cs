using Collectables;
using DataPersistence;
using UnityEngine;

namespace Player.PlayerData
{
    public class EatablesCount : MonoBehaviour, IDataPersistence
    {
        private int _eatableCount;

        private void OnEnable()
        {
            EatableCounterTrigger.OnEatableCountChangedEvent += IncrementEatableCount;
        }
        
        private void OnDisable()
        {
            EatableCounterTrigger.OnEatableCountChangedEvent -= IncrementEatableCount;
        }
        
        private void IncrementEatableCount()
        {
            _eatableCount++;
        }
        
        public int GetCount()
        {
            return _eatableCount;
        }

        public void LoadData(GameData data)
        {
           _eatableCount = data.eatableCount;
        }

        public void SaveData(GameData data)
        {
            data.eatableCount = _eatableCount;
        }
    }
}