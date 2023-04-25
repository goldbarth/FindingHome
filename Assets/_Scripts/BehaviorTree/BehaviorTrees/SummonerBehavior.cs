using System;
using UnityEngine;

namespace BehaviorTree.BehaviorTrees
{
    public class SummonerBehavior : MonoBehaviour
    {
        [SerializeField] private string _targetID;
        
        [ContextMenu("Generate guid for target id")]
        private void GenerateGuid()
        {
            _targetID = Guid.NewGuid().ToString();
        }
    }
}