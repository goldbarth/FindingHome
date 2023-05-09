using UnityEngine;

namespace BehaviorTree.NPCStats
{
    public class BTStats : ScriptableObject
    {
        [Header("Layer Masks")]
        public LayerMask TargetLayer;
        public LayerMask PlayerLayer;
        [Header("Tags")]
        public string PlayerTag = "player";
        public string TargetTag = "target";
    }
}