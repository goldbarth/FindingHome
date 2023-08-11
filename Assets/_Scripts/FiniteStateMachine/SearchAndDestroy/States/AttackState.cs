using FiniteStateMachine.SearchAndDestroy.Base;
using BehaviorTree.Blackboard;
using BehaviorTree.NPCStats;
using Enemies.Summoner;
using UnityEngine;
using AddIns;

namespace FiniteStateMachine.SearchAndDestroy.States
{
    public class AttackState : State
    {
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly SpitterStats _stats;
        private readonly Animator _animator;
        private readonly Rigidbody2D _rigid;
        
        private Vector2 _velocity;
        private Summoner _enemy;
        
        private float _attackCounter;
        private bool _isAttacking;

        public AttackState(SpitterStats stats, Transform transform, Blackboard blackboard)
        {
            _transform = transform.parent;
            _rigid = _transform.GetComponentInChildren<Rigidbody2D>();
            _animator = _transform.GetComponentInChildren<Animator>();
            _blackboard = blackboard;
            _attackCounter = 0f;
            _stats = stats;
        }
        
        protected override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("AttackState OnEnter");
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (!_isAttacking && OnCanTransitionTo())
                AttackPhase();
            if (_isAttacking)
                AttackPhase();
        }

        protected override void OnExit()
        {
            base.OnExit();
            Debug.Log("AttackState OnExit");
        }
        
        #region Attack related methods
        
        private void AttackPhase()
        {
            if (!_blackboard.ContainsKey(_stats.TargetTag))
            {
                Debug.Log("No target found");
                return;
            }

            var target = SetEnemy();
            if (!IsDistanceLessThanAttackRange(target))
                _transform.position = Vector2.SmoothDamp(_transform.position, target.position, ref _velocity,
                    _stats.SmoothTimeFast);
            
            SetLookDirection(target);

            if (!IsInAttackPhase()) return;
            
            if (_enemy.TakeDamage(_stats.AttackDamage))
                EnemyIsDead();
        }
        
        private bool IsInAttackPhase()
        {
            if (_attackCounter < _stats.AttackTimeTest)
            {
                _attackCounter += Time.deltaTime;
                _isAttacking = true;
                return false;
            }

            _attackCounter = 0f;
            _animator.SetBool("IsAttacking", true);
            return true;
        }

        private void EnemyIsDead()
        {
            _blackboard.ClearData(_stats.TargetTag);
            _animator.SetBool("IsAttacking", false);
            
            _isAttacking = false;
            
            StateController.AttackState.OnStateExit();
        }
        
        private void SetLookDirection(Transform target)
        {
            var direction = Vec2.Direction(_transform.position, target.position);
            Vec2.LookAt(_rigid, direction);
        }

        #endregion
        
        #region Transition related methods
        
        protected override bool OnCanTransitionTo()
        {
            var target = SetTarget();
            if (target != null && IsDistanceLessThanAttackRange(target))
            {
                _isAttacking = true;
                return true;
            }
            
            return false;
        }

        private bool IsDistanceLessThanAttackRange(Transform target)
        {
            var distance = Vector2.Distance(_transform.position, target.position);
            return distance < _stats.AttackRadius;
        }
        
        #endregion
        
        private Transform SetTarget()
        {
            return _blackboard.GetData<Transform>(_stats.TargetTag);
        }

        private Transform SetEnemy()
        {
            var target = _blackboard.GetData<Transform>(_stats.TargetTag);
            _enemy = target.GetComponentInChildren<Summoner>();
            return target;
        }
    }
}