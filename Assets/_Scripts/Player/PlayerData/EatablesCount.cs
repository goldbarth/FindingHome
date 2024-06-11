using FiniteStateMachine.FollowPlayer.States;
using System.Collections;
using Collectibles;
using DataPersistence;
using UnityEngine;

namespace Player.PlayerData
{
    public class EatablesCount : MonoBehaviour, IDataPersistence
    {
        private int _previousEatableCount;
        private bool _hasEatableDecreased = false;
        private bool _isDataLoaded = false;
        private int _eatableCount;

        private void OnEnable()
        {
            EdibleCounterTrigger.OnEdibleCollectEvent += IncrementEdibleCount;
            EatEdibleState.OnConsumeEdible += DecrementEdibleCount;
        }

        private void OnDisable()
        {
            EdibleCounterTrigger.OnEdibleCollectEvent -= IncrementEdibleCount;
            EatEdibleState.OnConsumeEdible -= DecrementEdibleCount;
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

        public int GetCount()
        {
            return _eatableCount;
        }

        private IEnumerator WaitForData()
        {
            while (!_isDataLoaded)
                yield return null;

            _previousEatableCount = _eatableCount;
        }

        private void IncrementEdibleCount()
        {
            _eatableCount++;
        }
        
        private void DecrementEdibleCount()
        {
            _eatableCount--;
        }

        public void LoadData(GameData data)
        {
           _eatableCount = data.edibleCount;
           _isDataLoaded = true;
        }

        public void SaveData(GameData data)
        {
            data.edibleCount = _eatableCount;
        }
    }
}