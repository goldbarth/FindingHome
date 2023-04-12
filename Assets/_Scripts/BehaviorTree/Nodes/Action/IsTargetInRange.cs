using System;
using UnityEngine;

namespace BehaviorTree.Nodes.Action
{
    public class IsTargetInRange : Node
    {
        [SerializeField] private string targetID;
        
        //TODO: Test purpose. Entity should be passed in from the tree.
        [SerializeField] private float detectionRadius = 10f;
        [SerializeField] private LayerMask layerMask = 0;
        [SerializeField] private new Transform transform;
        
        public IsTargetInRange(Transform transform)
        {
            this.transform = transform;
        }
        
        [ContextMenu("Generate guid for target id")]
        private void GenerateGuid()
        {
            targetID = Guid.NewGuid().ToString();
        }

        public override ReturnStat Tick()
        {
            var summoner = GetData(targetID);
            if (summoner == null)
            {
                var colliders = Physics.OverlapSphere(transform.position, detectionRadius, layerMask);
                if(colliders.Length > 0)
                {
                    Parent.Parent.SetData(targetID, colliders[0].transform);
                    Result = ReturnStat.SUCCESS;
                    return Result;
                }
            
                Result = ReturnStat.FAILURE;
                return Result;
            }
            else
            {
                var colliders = Physics.OverlapSphere(transform.position, detectionRadius, layerMask);
                if(colliders.Length == 0)
                {
                    Parent.Parent.ClearData(targetID);
                    Result = ReturnStat.FAILURE;
                    return Result;
                }
            
                Result = ReturnStat.SUCCESS;
                return Result;
            }
        }
    }
}