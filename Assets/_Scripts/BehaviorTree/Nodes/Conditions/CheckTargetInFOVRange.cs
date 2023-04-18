using UnityEngine;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckTargetInFOVRange : Node
    {
        //TODO: Test purpose. Entity should be passed in from the tree.
        private float _detectionRadius;
        private Transform _transform;
        private LayerMask _layerMask;
        private Animator _animator;
        
        public CheckTargetInFOVRange(float detectionRadius, LayerMask layerMask, Transform transform)
        {
            _detectionRadius = detectionRadius;
            _transform = transform;
            _layerMask = layerMask;
            _animator = transform.GetComponentInChildren<Animator>();
        }

        public override NodeState Evaluate()
        {
            var target = GetData("target");
            if (target == null)
            {
                var colliders = Physics2D.OverlapCircleAll(_transform.position, _detectionRadius, _layerMask);

                if (colliders.Length > 0)
                {
                    Parent.Parent.SetData("target", colliders[0].transform);
                    State = NodeState.SUCCESS;
                    return State;
                }

                State = NodeState.FAILURE;
                return State;
            }
            else
            {
                var colliders = Physics2D.OverlapCircleAll(_transform.position, _detectionRadius, _layerMask);
                if (colliders.Length == 0)
                {
                    Parent.Parent.ClearData("target");
                    _animator.SetBool("IsAttacking", false);
                    _animator.SetBool("IsWalking", false);
                    State = NodeState.FAILURE;
                    return State;
                }

                State = NodeState.SUCCESS;
                return State;
            }
        }
    }
}