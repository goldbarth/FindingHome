﻿using BehaviorTree.Blackboard;
using BehaviorTree.NPCStats;
using BehaviorTree.Core;
using UnityEngine;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckIfTargetInAttackRange : ConditionNode
    {
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly SpitterStats _stats;
        private readonly float _attackRange;
        
        public CheckIfTargetInAttackRange(SpitterStats stats, Transform transform, IBlackboard blackboard)
        {
            _attackRange = stats.AttackRadius;
            _transform = transform.parent;
            _blackboard = blackboard;
            _stats = stats;
        }

        public override NodeState Evaluate()
        {
            var target = _blackboard.GetData<Transform>("target");
            var distance = Vector2.Distance(_transform.position, target.position);
            if (distance < _attackRange)
            {
                _stats.IsInAttackPhase = true;

                State = NodeState.Success;
                return State;
            }
            
            _stats.IsInAttackPhase = false;
            
            State = NodeState.Failure;
            return State;
        }
    }
}