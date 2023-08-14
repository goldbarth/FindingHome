using BehaviorTree.Blackboard;
using HelpersAndExtensions;
using Enemies.Summoner;
using FiniteStateMachine.Base;
using NpcSettings;
using UnityEngine;

namespace FiniteStateMachine.ChaseAndAttack.States
{
    public class AttackState : State
    {
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly Animator _animator;
        private readonly Rigidbody2D _rigid;
        private readonly NpcData _stats;
        
        private Transform _target;
        private Vector2 _velocity;
        private Summoner _enemy;
        
        private float _attackCounter;

        public AttackState(NpcData stats, Transform transform, IBlackboard blackboard)
        {
            _transform = transform.parent;
            _rigid = _transform.GetComponentInChildren<Rigidbody2D>();
            _animator = _transform.GetComponentInChildren<Animator>();
            _blackboard = blackboard;
            _attackCounter = 0f;
            _stats = stats;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            Attack();
        }
        
        private void Attack()
        {
            if (!_blackboard.ContainsKey(_stats.TargetTag))
                return;

            _target = GetTarget();
            if (_target == null) return;
            
            if (!IsDistanceLessThanAttackRange() && !IsDistanceLessThanStopRange())
                _transform.position = Vector2.SmoothDamp(_transform.position, _target.position, ref _velocity,
                    _stats.SmoothTimeFast);
            
            SetLookDirection();

            if (!IsInAttackPhase()) return;
            
            _enemy = GetEnemy();
            if (_enemy.TakeDamage(_stats.AttackDamage))
                EnemyIsDead();
        }
        
        private bool IsInAttackPhase()
        {
            if (_attackCounter < _stats.AttackTimeFSM)
            {
                _attackCounter += Time.deltaTime;
                _stats.IsInAttackPhase = true;
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
            
            _stats.IsInAttackPhase = false;
            
            StateController.AttackState.OnStateExit();
        }
        
        private void SetLookDirection()
        {
            var direction = Vec2.Direction(_transform.position, _target.position);
            Vec2.LookAt(_rigid, direction);
        }
        
        private bool IsDistanceLessThanStopRange()
        {
            var distance = Vector2.Distance(_transform.position, _target.position);
            return distance < _stats.TargetStopDistance;
        }

        private bool IsDistanceLessThanAttackRange()
        {
            var distance = Vector2.Distance(_transform.position, _target.position);
            if (!(distance < _stats.AttackRadius)) return false;
            
            _stats.IsInAttackPhase = true;
            return true;
        }

        private Summoner GetEnemy()
        { 
            return GetTarget().GetComponentInChildren<Summoner>();
        }
        
        private Transform GetTarget()
        {
            return _blackboard.GetData<Transform>(_stats.TargetTag);
        }
    }
}