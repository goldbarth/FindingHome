using System.Collections.Generic;
using BehaviorTree.Nodes.Action;
using BehaviorTree.Nodes.Composite;
using UnityEngine;

namespace BehaviorTree.Behaviors
{
    public class SpitterBehavior : Tree
    {
        //TODO: testing purpose. entities later.
        [SerializeField] private new Transform transform;

        private SummonerBehavior _summoner;
        
        protected override Node CreateTree()
        {
            var root = new Selector(new List<Node>
            {
                new Sequence(new List<Node>
                {
                    new IsTargetInRange(transform, _summoner.TargetID),
                    
                })
            });
            
            return root;
        }
    }
}