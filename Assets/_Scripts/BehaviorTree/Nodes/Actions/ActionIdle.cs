using BehaviorTree.Blackboard;
using BehaviorTree.NPCStats;
using BehaviorTree.Core;
using UnityEngine;
using System;
using AddIns;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionIdle : ActionNode
    {
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly SpitterStats _stats;
        private readonly Rigidbody2D _rigid;
        private readonly Animator _animator;
        
        private Vector2 _previousPosition;
        private float _range;

        public ActionIdle(Enum rangeType, SpitterStats stats, Transform transform, Animator animator, IBlackboard blackboard)
        {
            switch (rangeType)
            {
                case RangeType.Near:
                    _range = stats.NearRangeStopDistance;
                    break;
                case RangeType.Far:
                    _range = stats.FarRangeStopDistance;
                    break;
                case RangeType.Protect:
                    _range = stats.ProtectRangeStopDistance;
                    break;
            }
            _rigid = transform.parent.GetComponent<Rigidbody2D>();
            _transform = transform.parent;
            _blackboard = blackboard;
            _animator = animator;
            _stats = stats;
        }
        
        public override NodeState Evaluate()
        {
            if (_stats.IsInAttackPhase)
            {
                State = NodeState.Failure;
                return State;
            }
            
            //_range = _stats.IsFarRange ? _stats.FarRangeStopDistance : _stats.ProtectRangeStopDistance;
            
            var player = _blackboard.GetData<Transform>(_stats.PlayerTag);
            var position = _transform.position;
            var direction = Vec2.Direction(position, player.position);
            
            if (Vec2.DistanceBetween(_stats, position, player.position, _range))
            {
                //TODO: random idle animations and/or behaviors, like looking around or jumping
                _animator.SetBool("IsWalking", false);
                Vec2.LookAt(_rigid, direction);
                
                Debug.Log("Idle");
                State = NodeState.Running;
                return State;
            }

            State = NodeState.Failure;
            return State;
        }
    }
}