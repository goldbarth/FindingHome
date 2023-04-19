using BehaviorTree.Nodes.Actions;
using Collectables;
using DataPersistence;
using UnityEngine;

namespace Player.PlayerData
{
    public class EatablesCount : MonoBehaviour, IDataPersistence
    {
        private int _eatableCount;
        private bool _hasEatableRemoved;

        private void OnEnable()
        {
            EatableCounterTrigger.OnEatableCountChangedEvent += IncrementEatableCount;
            ActionConsumeEatable.OnConsumeEatableEvent += DecrementEatableCount;
        }

        private void OnDisable()
        {
            EatableCounterTrigger.OnEatableCountChangedEvent -= IncrementEatableCount;
            ActionConsumeEatable.OnConsumeEatableEvent -= DecrementEatableCount;
        }
        
        private void IncrementEatableCount()
        {
            _eatableCount++;
        }
        
        private void DecrementEatableCount()
        {
            _eatableCount--;
            _hasEatableRemoved = true;
        }
        
        public int GetCount()
        {
            return _eatableCount;
        }
        
        public bool HasEatableRemoved()
        {
            if(_hasEatableRemoved)
            {
                _hasEatableRemoved = false;
                return true;
            }
            
            return false;
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