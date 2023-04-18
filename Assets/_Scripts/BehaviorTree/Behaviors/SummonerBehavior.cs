using System;
using UnityEngine;

namespace BehaviorTree.Behaviors
{
    public class SummonerBehavior : MonoBehaviour
    {
        [SerializeField] private string targetID;
        
        [ContextMenu("Generate guid for target id")]
        private void GenerateGuid()
        {
            targetID = Guid.NewGuid().ToString();
        }
    }
}