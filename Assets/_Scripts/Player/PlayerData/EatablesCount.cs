using System;
using System.Collections;
using BehaviorTree.Nodes.Actions;
using Collectables;
using DataPersistence;
using UnityEngine;

namespace Player.PlayerData
{
    public class EatablesCount : MonoBehaviour, IDataPersistence, IEatables
    {
        private int _previousEatableCount;
        private bool _hasEatableDecreased = false;
        private bool _isDataLoaded = false;
        private int _eatableCount;

        private void OnEnable()
        {
            EatableCounterTrigger.OnEatableCollectEvent += IncrementEatableCount;
            ActionConsumeEatable.OnConsumeEatableEvent += DecrementEatableCount;
        }

        private void OnDisable()
        {
            EatableCounterTrigger.OnEatableCollectEvent -= IncrementEatableCount;
            ActionConsumeEatable.OnConsumeEatableEvent -= DecrementEatableCount;
        }

        private void Start()
        {
            StartCoroutine(WaitForData());
        }

        private void Update()
        {
            EatableDecreaseCheck();
        }

        private void EatableDecreaseCheck()
        {
            _hasEatableDecreased = _eatableCount < _previousEatableCount;
            _previousEatableCount = _eatableCount;
        }
        
        public bool HasEatableDecreased()
        {
            var result = _hasEatableDecreased;
            _hasEatableDecreased = false;
            return result;
        }
        
        public bool HasEatable()
        {
            return _eatableCount > 0;
        }
        
        public int GetCount()
        {
            return _eatableCount;
        }

        private IEnumerator WaitForData()
        {
            while (!_isDataLoaded)
            {
                yield return null;
            }

            _previousEatableCount = _eatableCount;
        }

        private void IncrementEatableCount()
        {
            _eatableCount++;
        }
        
        private void DecrementEatableCount()
        {
            _eatableCount--;
        }

        public void LoadData(GameData data)
        {
           _eatableCount = data.eatableCount;
           _isDataLoaded = true;
        }

        public void SaveData(GameData data)
        {
            data.eatableCount = _eatableCount;
        }
    }
}