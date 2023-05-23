﻿using BehaviorTree.Blackboard;
using BehaviorTree.NPCStats;
using BehaviorTree.Core;
using Enemies.Summoner;
using UnityEngine;
using AddIns;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionAttackTarget : ActionNode
    {
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly SpitterStats _stats;
        private readonly Rigidbody2D _rigid;
        private readonly Animator _animator;
        private readonly int _attackDamage;

        private Transform _lastTarget;
        private Summoner _summoner;
        private Transform _target;
        private float _attackCounter;
        
        public ActionAttackTarget(SpitterStats stats, Transform transform, Animator animator, IBlackboard blackboard)
        {
            _rigid = transform.parent.GetComponent<Rigidbody2D>();
            _transform = transform.parent;
            _blackboard = blackboard;
            _animator = animator;
            _attackCounter = 0f;
            _stats = stats;
        }
        
        public override NodeState Evaluate()
        {
            if (!_blackboard.ContainsKey(_stats.TargetTag))
            {
                State = NodeState.Failure;
                return State;
            }

            var target = _blackboard.GetData<Transform>(_stats.TargetTag);

            _summoner = target.GetComponentInChildren<Summoner>();

            var direction = Vec2.Direction(_transform.position, target.position);
            Vec2.LookAt(_rigid, direction);

            if (_attackCounter < _stats.AttackTime)
            {
                _attackCounter += Time.deltaTime;
                _stats.IsInAttackPhase = true;
                
                State = NodeState.Running;
                return State;
            }

            _attackCounter = 0f;

            _animator.SetBool("IsAttacking", true);

            var enemyIsDead = _summoner.TakeDamage(_stats.AttackDamage);
            if (enemyIsDead)
            {
                _stats.IsInAttackPhase = false;
                _blackboard.ClearData(_stats.TargetTag);
                _animator.SetBool("IsAttacking", false);

                State = NodeState.Failure;
                return State;
            }

            State = NodeState.Running;
            return State;
        }
    }
}