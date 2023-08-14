using UnityEditor.Animations;
using UnityEngine;

namespace NpcSettings
{
    [CreateAssetMenu(fileName = "NpcData", menuName = "ScriptableObjects/BehaviorTree/NPC", order = 1)]
    public class NpcData : ScriptableObject
    {
        [Header("Layer Masks")]
        public LayerMask TargetLayer;
        public LayerMask PlayerLayer;
        [Header("Tags")]
        public string PlayerTag = "player";
        public string TargetTag = "target";
        [Header("Components")]
        [Tooltip("Get switched when becomes friendly.")]
        public AnimatorController AnimatorController;
        [Header("Radii")]
        [Range(.5f, 10f)]
        public float DetectionRadiusEnemy = 4f;
        [Range(.5f, 10f)]
        public float DetectionRadiusPlayer = 8f;
        [Range(.5f, 10f)]
        public float AttackRadius = 2f;
        [Header("Distances")]
        [Range(.5f, 10f)]
        public float StopDistanceEat = 1.2f;
        [Range(.5f, 10f)]
        public float ProtectRangeStopDistance = 6.2f;
        [Tooltip("Increase the amount to get a greater distance to the object, and reverse. A good approach is to start with 3.")]
        [Range(.5f, 10f)]
        public float NearRangeStopDistance = 4f;
        [Tooltip("Increase the amount to get a greater distance to the object, and reverse. A good approach is to start with 1.")]
        [Range(.5f, 10f)]
        public float FarRangeStopDistance= 1.5f;
        [Range(- 5f, 10f)]
        public float MinBackupDistance = 3f;
        [Range(- 5f, 10f)]
        public float MaxBackupDistance = 3f;
        [Range(.5f, 10f)]
        public float TargetStopDistance = .5f;
        [Range(0f, 10f)]
        public float DistanceBetweenOffset = 0.0001f;
        [Header("Attack Stats")]
        [Range(.001f, 10f)]
        public float AttackTimeFSM = .7f;
        [Range(.001f, 3f)]
        public float AttackTime = .7f;
        [Range(0.1f, 3f)]
        public float EnemyAnimationHitDelay = .33f;
        [Range(1, 30)]
        public int AttackDamage = 10;
        [Header("Smooth Times, Speeds, Forces")]
        [Range(.01f, 10f)]
        public float SmoothTime = 1.8f;
        [Range(.01f, 10f)]
        public float SmoothTimeFast = .8f;
        [Range(.01f, 10f)]
        public float SmoothTimeBackup = .47f;
        [Range(.01f, 10f)]
        public float JumpForce = 1.5f;
        [Header("Flags")]
        public bool HasEaten;
        public bool IsFarRange;
        public bool IsInAttackPhase;
        public bool IsInChasePhase;
        public bool HasBackedUp;
        [Header("Gizmos")]
        public bool ShowEnemyDetectionRadius = true;
        public bool ShowPlayerDetectionRadius = true;
        public bool ShowBackupRadius = true;
        public bool ShowFollowRadius = true;
        public bool ShowIdleSpace = true;
    }
}