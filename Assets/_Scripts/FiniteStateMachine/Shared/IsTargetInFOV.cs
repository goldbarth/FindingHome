using FiniteStateMachine.Base;
using BehaviorTree.Blackboard;
using Enemies.Summoner;
using NpcSettings;
using UnityEngine;
using System;

namespace FiniteStateMachine.Shared
{
    public class IsTargetInFOV : Transition
        {
            private readonly IBlackboard _blackboard;
            private readonly Transform _transform;
            private readonly LayerMask _layerMask;
            private readonly NpcData _stats;
            
            private readonly string _playerID = string.Empty;
            private readonly float _detectionRadius;
            private readonly string _key;
            
            public IsTargetInFOV(Enum targetType, NpcData stats, Transform transform, IBlackboard blackboard)
            {
                switch (targetType)
                {
                    case TargetType.Player:
                        _key = stats.PlayerTag;
                        _detectionRadius = stats.DetectionRadiusPlayer;
                        _layerMask = stats.PlayerLayer;
                        break;
                    case TargetType.Enemy:
                        _key = stats.TargetTag;
                        _detectionRadius = stats.DetectionRadiusEnemy;
                        _layerMask = stats.TargetLayer;
                        break;
                }
                
                _transform = transform.parent;
                _blackboard = blackboard;
                _stats = stats;
            }

            public override bool OnCanTransitionTo()
            {
                var target = GetTarget();
                if (target == null)
                {
                    // ReSharper disable once Unity.PreferNonAllocApi
                    var colliders = Physics2D.OverlapCircleAll(_transform.position, _detectionRadius, _layerMask);
                    if (IsColliderDetected(colliders))
                    {
                        SetTargetObject(colliders);
                        return true;
                    }
                }
                else
                {
                    // ReSharper disable once Unity.PreferNonAllocApi
                    var colliders = Physics2D.OverlapCircleAll(_transform.position, _detectionRadius, _layerMask);
                    if (IsNoColliderDetected(colliders))
                    {
                        _blackboard.ClearData(_key);
                        return false;
                    }
                }

                return true;
            }

            private object GetTarget()
            {
                return _blackboard.GetData<object>(_key);
            }

            private static bool IsNoColliderDetected(Collider2D[] colliders)
            {
                return colliders.Length == 0;
            }

            private static bool IsColliderDetected(Collider2D[] colliders)
            {
                return colliders.Length > 0;
            }

            private void SetTargetObject(Collider2D[] colliders)
            {
                if (_key == _stats.PlayerTag)
                    _blackboard.SetData(_key, _playerID, colliders[0].transform);
                if (_key == _stats.TargetTag)
                {
                    var enemy = colliders[0].GetComponentInChildren<Summoner>();
                    if (enemy != null)
                        _blackboard.SetData(_key, enemy._id, enemy.transform.parent);
                }
            }
        }
}