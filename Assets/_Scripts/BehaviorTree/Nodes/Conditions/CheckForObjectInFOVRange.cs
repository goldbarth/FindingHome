using BehaviorTree.Blackboard;
using BehaviorTree.Core;
using UnityEngine;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckForObjectInFOVRange : ConditionNode
    {
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly LayerMask _layerMask;
        private readonly float _detectionRadius;
        private readonly string _key;

        public CheckForObjectInFOVRange(string key, float radius, LayerMask layerMask, Transform transform, IBlackboard blackboard)
        {
            _detectionRadius = radius;
            _blackboard = blackboard;
            _transform = transform;
            _layerMask = layerMask;
            _key = key;
        }

        public override NodeState Evaluate()
        {
            var obj = _blackboard.GetData<object>(_key);
            if (obj is null)
            {
                var colliders = Physics2D.OverlapCircleAll(_transform.position, _detectionRadius, _layerMask);

                if (colliders.Length > 0)
                {
                    _blackboard.SetData(_key, colliders[0].transform);
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
                    _blackboard.ClearData(_key);
                    State = NodeState.Failure;
                    return State;
                }

                State = NodeState.Success;
                return State;
            }
        }
    }
}