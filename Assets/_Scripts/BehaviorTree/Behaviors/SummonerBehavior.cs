using System;
using UnityEngine;

namespace BehaviorTree.Behaviors
{
    public class SummonerBehavior : Tree
    {
        [SerializeField] private string _targetID;

        public string TargetID => _targetID;

        [ContextMenu("Generate guid for target id")]
        private void GenerateGuid()
        {
            _targetID = Guid.NewGuid().ToString();
        }
        
        //TODO: testing purpose. entities later.
        [SerializeField] private new Transform transform;

        protected override Node CreateTree()
        {
            return null;
        }
    }
}