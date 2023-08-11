using BehaviorTree.Blackboard;
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
            var target = SetTarget();
            if (IsDistanceLessThanAttackRange(target))
            {
                State = NodeState.Success;
                return State;
            }

            State = NodeState.Failure;
            return State;
        }

        private Transform SetTarget()
        {
            return _blackboard.GetData<Transform>(_stats.TargetTag);
        }
        
        private bool IsDistanceLessThanAttackRange(Transform target)
        {
            var distance = Vector2.Distance(_transform.position, target.position);
            return distance < _attackRange;
        }
    }
}