using UnityEngine;

namespace BehaviorTree.Nodes.Conditions
{
    public enum CheckType
    {
        TargetInFOVRange,
        PlayerInFOVRange,
        PlayerHasEatable,
    }
    public class CheckTargetInFOVRange : Node
    {
        //TODO: Test purpose. Entity should be passed in from the tree.
        private float _detectionRadius;
        private bool _hasEaten = false;
        private Transform _transform;
        private CheckType _checkType;
        private LayerMask _layerMask;
        private Animator _animator;
        
        
        public CheckTargetInFOVRange(float detectionRadius, LayerMask layerMask, Transform transform, CheckType checkType ,bool hasEaten)
        {
            _animator = transform.GetComponentInChildren<Animator>();
            _detectionRadius = detectionRadius;
            _checkType = checkType;
            _transform = transform;
            _layerMask = layerMask;
            _hasEaten = hasEaten;
        }

        public override NodeState Evaluate()
        {
            if (_checkType == CheckType.PlayerInFOVRange && _hasEaten)
            {
                State = NodeState.Failure;
                return State;
            }

            var target = GetData("target");
            if (target == null)
            {
                var colliders = Physics2D.OverlapCircleAll(_transform.position, _detectionRadius, _layerMask);

                if (colliders.Length > 0)
                {
                    Parent.Parent.SetData("target", colliders[0].transform);
                    State = NodeState.Success;
                    return State;
                }

                State = NodeState.Failure;
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
                    State = NodeState.Failure;
                    return State;
                }

                State = NodeState.Success;
                return State;
            }
        }
    }
}