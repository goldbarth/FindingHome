using System;
using UnityEngine;

namespace BehaviorTree.Nodes.Action
{
    public class IsSummonerInRange : Node
    {
        [SerializeField] private float detectionRadius = 10f;
        
        //TODO: Test purpose. Entity should be passed in from the tree.
        [SerializeField] private LayerMask layerMask = 0;
        [SerializeField] private Transform summonerTransform;


        public override ReturnStat Tick()
        {
            //var summoner = GetData("Summoner");
            //if (summoner == null)
            //{ } else{ }
            
            var colliders = Physics.OverlapSphere(transform.position, detectionRadius, layerMask);
                
            if(colliders.Length > 0)
            {
                //Parent.Parent.SetData("Summoner", colliders[0].transform);
                Result = ReturnStat.SUCCESS;
                return Result;
            }
            
            Result = ReturnStat.FAILURE;
            return Result;
        }
    }
}