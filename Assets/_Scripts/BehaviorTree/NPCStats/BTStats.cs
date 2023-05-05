using UnityEngine;

namespace BehaviorTree.NPCStats
{
    public class BTStats : ScriptableObject
    {
        [Header("Layer Masks")]
        public LayerMask _targetLayer;
        public LayerMask _playerLayer;
        [Header("Tags")]
        public string _playerTag = "player";
        public string _targetTag = "target";
    }
}