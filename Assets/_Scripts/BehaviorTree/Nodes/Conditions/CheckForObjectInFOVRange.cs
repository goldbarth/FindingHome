using BehaviorTree.Blackboard;
using BehaviorTree.NPCStats;
using BehaviorTree.Core;
using UnityEngine;
using Enemies;
using System;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckForObjectInFOVRange : ConditionNode
    {
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly LayerMask _layerMask;
        private readonly SpitterStats _stats;
        private readonly float _detectionRadius;
        private readonly string _key;

        public CheckForObjectInFOVRange(Enum fovType, SpitterStats stats, Transform transform, IBlackboard blackboard)
        {
            switch (fovType)
            {
                case FOVType.Player:
                    _key = stats.PlayerTag;
                    _detectionRadius = stats.DetectionRadiusPlayer;
                    _layerMask = stats.PlayerLayer;
                    break;
                case FOVType.Target:
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
                    if(_key == _stats.PlayerTag)
                        _blackboard.SetData(_key, string.Empty, colliders[0].transform);
                    if (_key == _stats.TargetTag)
                    {
                        if(colliders[0].TryGetComponent(out Summoner target))
                            _blackboard.SetData(_key, target._id, colliders[0].transform);
                    }
                    
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
    }
}