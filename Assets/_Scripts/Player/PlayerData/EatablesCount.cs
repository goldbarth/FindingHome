using System;
using System.Collections;
using BehaviorTree.Nodes.Actions;
using Collectables;
using DataPersistence;
using UnityEngine;

namespace Player.PlayerData
{
    public class EatablesCount : MonoBehaviour, IDataPersistence
    {
        private int _eatableCount;
        private int _previousEatableCount;
        private bool _hasEatableDecreased = false;
        private bool _isDataLoaded = false;

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
            if (_eatableCount < _previousEatableCount)
            {
                _hasEatableDecreased = true;
            }
            else
            {
                _hasEatableDecreased = false;
            }
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
        
        public int GetCount()
        {
            return _eatableCount;
        }
        
        public bool HasEatableDecreased()
        {
            return _hasEatableDecreased;
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