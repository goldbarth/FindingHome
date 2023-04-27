using UnityEngine;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckIfTargetInAttackRange : LeafNode
    {
        private readonly Transform _transform;
        private readonly Animator _animator;
        private readonly float _attackRange;

        public CheckIfTargetInAttackRange(Transform transform, float attackRange)
        {
            _animator = transform.GetComponentInChildren<Animator>();
            _transform = transform;
            _attackRange = attackRange;
        }

        public override NodeState Evaluate()
        {
            var obj = GetData("target");
            if (obj == null)
            {
                State = NodeState.Failure;
                return State;
            }
            
            var target = (Transform)obj;
            var distance = Vector2.Distance(_transform.position, target.position);
            if (distance < _attackRange)
            {
                Debug.Log("Target in attack range");
                _animator.SetBool("IsAttacking", true);
                
                State = NodeState.Success;
                return State;
            }
            
            State = NodeState.Failure;
            return State;
        }
    }
}