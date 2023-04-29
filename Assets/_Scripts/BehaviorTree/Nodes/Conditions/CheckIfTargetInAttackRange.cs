using BehaviorTree.Blackboard;
using BehaviorTree.Core;
using UnityEngine;
using Tree = BehaviorTree.Core.Tree;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckIfTargetInAttackRange : ConditionNode
    {
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly Animator _animator;
        private readonly float _attackRange;
        
        public static bool IsInAttackRange { get; private set; }

        public CheckIfTargetInAttackRange(Transform transform, float attackRange, IBlackboard blackboard)
        {
            _animator = transform.GetComponentInChildren<Animator>();
            _attackRange = attackRange;
            _blackboard = blackboard;
            _transform = transform;
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
                IsInAttackRange = true;
                Debug.Log("Target in attack range");
                _animator.SetBool("IsAttacking", true);
                
                State = NodeState.Success;
                return State;
            }
            
            IsInAttackRange = false;
            
            State = NodeState.Failure;
            return State;
        }
    }
}