using UnityEngine;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckPlayerInFOVRange : LeafNode
    {
        //TODO: Test purpose. Entity should be passed in from the tree.
        private float _detectionRadius;
        private Transform _transform;
        private LayerMask _layerMask;


        public CheckPlayerInFOVRange(float detectionRadius, LayerMask layerMask, Transform transform)
        {
            _detectionRadius = detectionRadius;
            _transform = transform;
            _layerMask = layerMask;
        }

        public override NodeState Evaluate()
        {
            var player = GetData("player");
            if (player is null)
            {
                var colliders = Physics2D.OverlapCircleAll(_transform.position, _detectionRadius, _layerMask);
                if (colliders.Length > 0)
                { 
                    Parent.SetData("player", colliders[0].transform);
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
                    Parent.ClearData("player");
                    State = NodeState.Failure;
                    return State;
                }

                State = NodeState.Success;
                return State;
            }
        }
    }
}