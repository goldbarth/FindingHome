using UnityEngine;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckForObjectInFOVRange : LeafNode
    {
        //TODO: Test purpose. Entity should be passed in from the tree.
        private readonly Transform _transform;
        private readonly LayerMask _layerMask;
        private readonly float _detectionRadius;
        private readonly string _targetKey;

        public CheckForObjectInFOVRange(string key, float radius, LayerMask layerMask, Transform transform)
        {
            _detectionRadius = radius;
            _transform = transform;
            _layerMask = layerMask;
            _targetKey = key;
        }

        public override NodeState Evaluate()
        {
            var obj = GetData(_targetKey);
            if (obj == null)
            {
                var colliders = Physics2D.OverlapCircleAll(_transform.position, _detectionRadius, _layerMask);

                if (colliders.Length > 0)
                {
                    Parent.SetData(_targetKey, colliders[0].transform);
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
                    Parent.ClearData(_targetKey);
                    State = NodeState.Failure;
                    return State;
                }

                State = NodeState.Success;
                return State;
            }
        }
    }
}