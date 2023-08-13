using System.Collections.Generic;
using System.Collections;
using NpcSettings;
using UnityEngine;
using System;

namespace Enemies.Summoner
{
    public class Summoner : Enemy
    {
        [Header("Spitter References")]
        [SerializeField] private NpcData _stats;
        [Header("Audio")]
        [SerializeField] private AudioSource _deathSound;
        [SerializeField] private AudioSource _summonSound;

        [Header("Random Animation")]
        [SerializeField] private float _minDelay;
        [SerializeField] private float _maxDelay;
        [Header("Patrol Waypoints")]
        [SerializeField] private Transform[] _waypoints;
        
        private Dictionary<SummonerAnimationEvents, Action>  _animationEventDictionary;
        private Animator _animator;
        private Vector2 _velocity;
        private int _currentWaypointIndex;
        private float _animationTimer;
        private bool _isAnimating;

        private void Awake()
        {
            _animator = transform.parent.GetComponentInChildren<Animator>();
        }

        private void Start()
        {
            _animationEventDictionary = new Dictionary<SummonerAnimationEvents, Action>
            {
                { SummonerAnimationEvents.DestroyGameObject, DestroySummoner },
                { SummonerAnimationEvents.FinishAnimation, AnimationFinished },
                { SummonerAnimationEvents.PlaySummonSound, PlaySummonSound },
                { SummonerAnimationEvents.PlayDeathSound, PlayDeathSound }
            };
        }

        private void Update()
        {
            Patrol();
            RandomAnimation();
        }

        protected internal override bool TakeDamage(int damage)
        {
            StartCoroutine(WaitTillGetHit());
            return base.TakeDamage(damage);
        }
        
        protected override void Die()
        {
            _animator.Play("sum_death");
        }
        
        private void Patrol()
        {
            if (_waypoints.Length == 0) return;

            var waypointPosition = _waypoints[_currentWaypointIndex].position;
            var newPosition = Vector2.SmoothDamp(transform.parent.position, waypointPosition, ref _velocity, _stats.SmoothTime);
            
            transform.parent.position = newPosition;
            _animator.SetBool("IsMoving", true);
            
            if(Vector2.Distance(newPosition, waypointPosition) < 0.1f)
                _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
        }

        private void RandomAnimation()
        {
            if (_isAnimating) return;
            
            _animationTimer -= Time.deltaTime;
            if (_animationTimer <= 0f)
            {
                PlayAnimation();
                _animationTimer = UnityEngine.Random.Range(_minDelay, _maxDelay);
            }
        }

        private void PlayAnimation()
        {
            _animator.SetTrigger("IsSummoning");
            _isAnimating = true;
        }

        public void CallAnimationEvent(SummonerAnimationEvents eventKey)
        {
            _animationEventDictionary[eventKey].Invoke();
        }

        private void DestroySummoner()
        {
            _stats.IsInAttackPhase = false;
            Destroy(transform.parent.gameObject);
        }

        private void AnimationFinished()
        {
            _isAnimating = false;
        }
        
        private void PlaySummonSound()
        {
            //Debug.Log("Summon sound");
            _summonSound.PlayOneShot(_summonSound.clip);
        }
        
        private void PlayDeathSound()
        {
            Debug.Log("Death sound");
            _deathSound.PlayOneShot(_deathSound.clip);
        }

        private IEnumerator WaitTillGetHit()
        {
            yield return new WaitForSeconds(_stats.EnemyAnimationHitDelay); //.33f
            _animator.SetTrigger("GetHit");
        }
    }
}