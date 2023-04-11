using System;
using UnityEngine;

namespace BehaviorTree.Enemies
{
    public class SummonerBehavior : Node
    {
        [SerializeField] private string id;
        
        private object _obj;
        private Node _sumNode;

        // generates an id for the item in scene
        [ContextMenu("Generate guid for id")]
        private void GenerateGuid()
        {
            id = Guid.NewGuid().ToString();
        }

        private void Start()
        {
            //_sumNode.SetData(id, _obj);
        }
    }
}