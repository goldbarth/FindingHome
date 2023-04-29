using UnityEditor.Animations;
using UnityEngine;

namespace BehaviorTree.Entity
{
    [CreateAssetMenu(fileName = "Entity", menuName = "ScriptableObjects/BehaviorTree/Entity", order = 1)]
    public class Entity : ScriptableObject
    {
        public LayerMask _targetLayer;
        public LayerMask _playerLayer;
        public GameObject _gameObject;
        public AnimatorController _friendlyAnimController;
        public string _playerTag = "player";
        public string _targetTag = "target";
        public float _detectionRadiusEnemy = 5f;
        public float _detectionRadiusPlayer = 7f;
        public float _attackRadius = 2f;
        public float _stopDistancePlayer = 3f;
        public float _stopDistancePlayerProtect = 1.8f;
        public float _speedGoToPlayer = 2.5f;
        public float _speedPlayerFollow = 6.8f;
        public float _speedTargetFollow = 5f;
    }
}