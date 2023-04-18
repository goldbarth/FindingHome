using UnityEngine;

namespace BehaviorTree.Nodes.Actions
{
    public class CheckTargetInAttackRange : LeafNode
    {
        private float _attackRange;
        private Transform _transform;
        private Animator _animator;

        public CheckTargetInAttackRange(Transform transform, float attackRange)
        {
            _animator = transform.GetComponentInChildren<Animator>();
            _transform = transform;
            _attackRange = attackRange;
        }

        public override NodeState Evaluate()
        {
            var target = GetData("target");
            if (target == null)
            {
                State = NodeState.FAILURE;
                return State;
            }

            var targetTransform = (Transform)target;
            if (Vector2.Distance(_transform.position, targetTransform.position) > _attackRange)
            {
                _animator.SetBool("IsAttacking", true);
                
                State = NodeState.SUCCESS;
                return State;
            }
            
            State = NodeState.FAILURE;
            return State;
        }
    }
}