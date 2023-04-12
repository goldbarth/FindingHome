using System;
using UnityEngine;

namespace BehaviorTree.Nodes.Action
{
    public class IsTargetInRange : Node
    {
        
        //TODO: Test purpose. Entity should be passed in from the tree.
        [SerializeField] private float detectionRadius = 10f;
        [SerializeField] private LayerMask layerMask = 0;
        private Transform _transform;
        private string _targetID;
        
        public IsTargetInRange(Transform transform, string target)
        {
            _transform = transform;
            _targetID = target;
        }

        public override NodeState Evaluate()
        {
            var target = GetData(_targetID);
            if (target == null)
            {
                var colliders = Physics.OverlapSphere(_transform.position, detectionRadius, layerMask);
                if(colliders.Length > 0)
                {
                    Parent.Parent.SetData(_targetID, colliders[0].transform);
                    Result = NodeState.SUCCESS;
                    return Result;
                }
            
                Result = NodeState.FAILURE;
                return Result;
            }
            else
            {
                var colliders = Physics.OverlapSphere(_transform.position, detectionRadius, layerMask);
                if(colliders.Length == 0)
                {
                    Parent.Parent.ClearData(_targetID);
                    Result = NodeState.FAILURE;
                    return Result;
                }
            
                Result = NodeState.SUCCESS;
                return Result;
            }
        }
    }
}