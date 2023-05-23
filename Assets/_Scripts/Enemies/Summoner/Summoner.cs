using System.Collections.Generic;
using UnityEngine;
using System;

namespace Enemies.Summoner
{
    public class Summoner : Enemy
    {
        private Dictionary<SummonerAnimationEvents, Action> _animationEventDictionary;
        private Animator _animator;

        private void Awake()
        {
            _animator = transform.parent.GetComponentInChildren<Animator>();
        }

        private void Start()
        {
            _animationEventDictionary = new Dictionary<SummonerAnimationEvents, Action>();
            _animationEventDictionary.Add(SummonerAnimationEvents.DestroyGameObject, DestroySummoner);
        }
        
        public void CallAnimationEvent(SummonerAnimationEvents eventKey)
        {
            _animationEventDictionary[eventKey].Invoke();
        }

        private void DestroySummoner()
        {
            Destroy(transform.parent.gameObject);
        }

        protected internal override bool TakeDamage(int damage)
        {
            _animator.SetTrigger("GetHit");
            return base.TakeDamage(damage);
        }

        protected override void Die()
        {
            _animator.Play("sum_death");
        }
    }
}