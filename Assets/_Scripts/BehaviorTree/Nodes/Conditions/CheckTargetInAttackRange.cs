using UnityEngine;

namespace BehaviorTree.Nodes.Conditions
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
            var target = (Transform)GetData("target");
            var distance = Vector2.Distance(_transform.position, target.position);
            if (distance < _attackRange)
            {
                Debug.Log("Target in attack range");
                State = NodeState.Success;
                return State;
            }
            
            State = NodeState.Failure;
            return State;
        }
    }
}