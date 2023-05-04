﻿using BehaviorTree.Blackboard;
using BehaviorTree.Core;
using UnityEngine;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckIfTargetInAttackRange : ConditionNode
    {
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly Animator _animator;
        private readonly float _attackRange;
        
        public CheckIfTargetInAttackRange(Transform transform, float attackRange, Animator animator, IBlackboard blackboard)
        {
            _animator = transform.parent.GetComponentInChildren<Animator>();
            _transform = transform.parent;
            _attackRange = attackRange;
            _blackboard = blackboard;
        }

        public override NodeState Evaluate()
        {
            var target = _blackboard.GetData<Transform>("target");
            if (target is null)
            {
                State = NodeState.Failure;
                return State;
            }

            var distance = Vector2.Distance(_transform.position, target.position);
            if (distance < _attackRange)
            {
                GameManager.Instance.IsInAttackPhase = true;

                State = NodeState.Success;
                return State;
            }
            
            GameManager.Instance.IsInAttackPhase = false;
            
            State = NodeState.Failure;
            return State;
        }
    }
}