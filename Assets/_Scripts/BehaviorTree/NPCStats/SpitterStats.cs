using UnityEditor.Animations;
using UnityEngine;

namespace BehaviorTree.NPCStats
{
    [CreateAssetMenu(fileName = "Stats", menuName = "ScriptableObjects/BehaviorTree/Stats", order = 1)]
    public class SpitterStats : BTStats
    {
        [Header("Components")]
        [Tooltip("Get switched when becomes friendly.")]
        public AnimatorController _animatorController;
        [Header("Radii")]
        [Range(.5f, 10f)]
        public float _detectionRadiusEnemy = 4f;
        [Range(.5f, 10f)]
        public float _detectionRadiusPlayer = 8f;
        [Range(.5f, 10f)]
        public float _attackRadius = 2f;
        [Header("Distances")]
        [Range(.5f, 10f)]
        public float _stopDistanceEat = 1.2f;
        [Range(.5f, 10f)]
        public float _stopDistancePlayerProtect = 1.8f;
        [Tooltip("Increase the amount to get a greater distance to the object, and reverse. A good approach is to start with 3.")]
        [Range(.5f, 10f)]
        public float _nearRangeStopDistance = 4f;
        [Tooltip("Increase the amount to get a greater distance to the object, and reverse. A good approach is to start with 1.")]
        [Range(.5f, 10f)]
        public float _farRangeStopDistance= 1.5f;
        [Range(.5f, 10f)]
        public float _backupDistance = 3f;
        [Range(.5f, 10f)]
        public float _targetStopDistance = .5f;
        [Range(.5f, 10f)]
        public float _distanceBetweenOffset = .5f;
        [Header("Speeds")]
        [Range(.5f, 10f)]
        public float _speedGoToPlayer = 6f;
        [Range(.5f, 10f)]
        public float _speedPlayerFollow = 6.8f;
        [Range(.5f, 10f)]
        public float _speedTargetFollow = 5f;
        [Header("Attack")]
        [Range(.001f, 3f)]
        public float _attackTime = .7f;
        [Range(1, 30)]
        public int _attackDamage = 10;
        [Header("Bools")]
        public bool _hasEaten;
        public bool _isInAttackPhase;
    }
}