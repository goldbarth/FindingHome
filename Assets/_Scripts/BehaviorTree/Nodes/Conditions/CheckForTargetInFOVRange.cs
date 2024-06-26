﻿using BehaviorTree.Blackboard;
using BehaviorTree.Core;
using Enemies.Summoner;
using NpcSettings;
using UnityEngine;
using System;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckForTargetInFOVRange : ConditionNode
    {
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly LayerMask _layerMask;
        private readonly NpcData _stats;
        
        private readonly string _playerID = string.Empty;
        private readonly float _detectionRadius;
        private readonly string _key;

        public CheckForTargetInFOVRange(Enum targetType, NpcData stats, Transform transform, IBlackboard blackboard)
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

        public override NodeState Evaluate()
        {
            var obj = _blackboard.GetData<object>(_key);
            if (obj is null)
            {
                var colliders = Physics2D.OverlapCircleAll(_transform.position, _detectionRadius, _layerMask);
                if (colliders.Length > 0)
                {
                    SetTargetObject(colliders);

                    State = NodeState.Success;
                    return State;
                }

                State = NodeState.Failure;
                return State;
            }
            else
            {
                var colliders = Physics2D.OverlapCircleAll(_transform.position, _detectionRadius, _layerMask);
                if (colliders.Length == 0)
                {
                    _blackboard.ClearData(_key);
                    
                    State = NodeState.Failure;
                    return State;
                }

                State = NodeState.Success;
                return State;
            }
        }

        private void SetTargetObject(Collider2D[] colliders)
        {
            if (_key == _stats.PlayerTag)
                _blackboard.SetData(_key, _playerID, colliders[0].transform);
            if (_key == _stats.TargetTag)
            {
                var enemy = colliders[0].GetComponentInChildren<Summoner>();
                if (enemy is not null)
                    _blackboard.SetData(_key, enemy._id, enemy.transform.parent);
            }
        }
    }
}