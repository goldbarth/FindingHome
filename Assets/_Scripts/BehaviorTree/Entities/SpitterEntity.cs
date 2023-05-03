using Player;
using UnityEngine;
using Player.PlayerData;
using UnityEditor.Animations;

namespace BehaviorTree.Entities
{
    [CreateAssetMenu(fileName = "Entity", menuName = "ScriptableObjects/BehaviorTree/Entity", order = 1)]
    public class SpitterEntity : Entity
    {
        [Header("Components")] 
        public Animator _animator;
        [Tooltip("Get switched when becomes friendly.")]
        public AnimatorController _animatorController;
        [Header("Radii")]
        [Range(.5f, 10f)]
        public float _detectionRadiusEnemy = 5f;
        [Range(.5f, 10f)]
        public float _detectionRadiusPlayer = 7f;
        [Range(.5f, 10f)]
        public float _attackRadius = 2f;
        [Header("Distances")]
        [Range(.5f, 10f)]
        public float _stopDistancePlayerProtect = 1.8f;
        [Tooltip("Increase the amount to get a greater distance to the object, and reverse. A good approach is to start with 3.")]
        [Range(.5f, 10f)]
        public float _nearRangeStopDistance = 5f;
        [Tooltip("Increase the amount to get a greater distance to the object, and reverse. A good approach is to start with 1.")]
        [Range(.5f, 10f)]
        public float _farRangeStopDistance= 1.5f;
        [Header("Speeds")]
        [Range(.5f, 10f)]
        public float _speedGoToPlayer = 2.5f;
        [Range(.5f, 10f)]
        public float _speedPlayerFollow = 6.8f;
        [Range(.5f, 10f)]
        public float _speedTargetFollow = 5f;
        
        public static PlayerController PlayerController => GetPlayerController();
        public static EatablesCount Eatables => GetEatablesCount();
        
        private static EatablesCount GetEatablesCount()
        {
            return GetPlayerController().GetComponent<EatablesCount>();
        }
        
        private static PlayerController GetPlayerController()
        {
            return FindObjectOfType<PlayerController>();
        }
    }
}