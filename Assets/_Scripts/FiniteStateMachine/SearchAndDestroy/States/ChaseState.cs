using FiniteStateMachine.SearchAndDestroy.Base;
using BehaviorTree.Blackboard;
using BehaviorTree.NPCStats;
using Enemies.Summoner;
using UnityEngine;
using AddIns;

namespace FiniteStateMachine.SearchAndDestroy.States
{
    public class ChaseState : State
    {
        private readonly AudioSource _audioSource;
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly LayerMask _layerMask;
        private readonly SpitterStats _stats;
        private readonly Animator _animator;
        private readonly Rigidbody2D _rigid;
        
        private readonly float _detectionRadius;

        private Transform _lastTarget;
        private Transform _target;

        private Vector2 _velocity;
        private bool _isChasing;

        public ChaseState(SpitterStats stats, Transform transform, IBlackboard blackboard, Animator animator,
            AudioSource audioSource)
        {
            _transform = transform.parent;
            _rigid = _transform.GetComponent<Rigidbody2D>();
            _detectionRadius = stats.DetectionRadiusEnemy;
            _layerMask = stats.TargetLayer;
            _audioSource = audioSource;
            _blackboard = blackboard;
            _animator = animator;
            _stats = stats;
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("ChaseState OnEnter");
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (!_isChasing && StateController.ChaseState.OnStateCanTransitionTo())
                ChasePhase();
            if (_isChasing)
                ChasePhase();
        }

        protected override void OnExit()
        {
            base.OnExit();
            Debug.Log("ChaseState OnExit");
        }

        #region Chase related methods

        private void ChasePhase()
        {
            if (!_blackboard.ContainsKey(_stats.TargetTag))
            {
                Debug.Log("Target is null");
                return;
            }

            var target = _blackboard.GetData<Transform>(_stats.TargetTag);
            if (target == null)
            {
                Debug.Log("Target is null");
                return;
            }

            var direction = Vec2.Direction(_transform.position, target.position);
            var distance = Vector2.Distance(_transform.position, target.position);

            if (distance > _stats.TargetStopDistance)
            {
                _transform.position = Vector2.SmoothDamp(_transform.position, target.position, ref _velocity,
                    _stats.SmoothTimeFast);
                Vec2.LookAt(_rigid, direction);

                _animator.SetBool("IsWalking", true);
                _audioSource.PlayOneShot(_audioSource.clip);
            }
            
            if (distance < _stats.TargetStopDistance)
            {
                _animator.SetBool("IsWalking", false);
                _audioSource.Stop();

                _isChasing = false;
                
                StateController.ChangeState(StateController.AttackState);
            }
        }

        #endregion

        #region Transition related methods

        protected override bool OnCanTransitionTo()
        {
            var targetObject = _blackboard.GetData<object>(_stats.TargetTag);
            if (targetObject is null)
            {
                // ReSharper disable once Unity.PreferNonAllocApi
                var colliders = Physics2D.OverlapCircleAll(_transform.position, _detectionRadius, _layerMask);
                if (colliders.Length > 0)
                {
                    SetTargetObject(colliders);
                    
                    _isChasing = true;
                    return true;
                }
            }
            else
            {
                // ReSharper disable once Unity.PreferNonAllocApi
                var colliders = Physics2D.OverlapCircleAll(_transform.position, _detectionRadius, _layerMask);
                if (colliders.Length == 0)
                {
                    _blackboard.ClearData(_stats.TargetTag);

                    return false;
                }
            }

            return false;
        }

        private void SetTargetObject(Collider2D[] colliders)
        {
            var enemy = colliders[0].GetComponentInChildren<Summoner>();
            if (enemy is not null)
                _blackboard.SetData(_stats.TargetTag, enemy._id, enemy.transform.parent);
        }

        #endregion
    }
}