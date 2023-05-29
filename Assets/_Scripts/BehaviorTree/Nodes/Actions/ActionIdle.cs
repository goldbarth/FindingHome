using BehaviorTree.Blackboard;
using BehaviorTree.NPCStats;
using BehaviorTree.Core;
using UnityEngine;
using System;
using AddIns;
using Player;
using Random = UnityEngine.Random;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionIdle : ActionNode
    {
        private readonly PlayerController _player;
        private readonly AudioSource _audioSource;
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly SpitterStats _stats;
        private readonly Rigidbody2D _rigid;
        private readonly Animator _animator;
        
        private Vector2 _previousPosition;
        private float _range;

        public ActionIdle(Enum rangeType, SpitterStats stats, PlayerController player, Transform transform, Animator animator, AudioSource audioSource, IBlackboard blackboard)
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
            _audioSource = audioSource;
            _blackboard = blackboard;
            _animator = animator;
            _player = player;
            _stats = stats;
        }
        
        public override NodeState Evaluate()
        {
            //_range = _stats.IsFarRange ? _stats.FarRangeStopDistance : _stats.ProtectRangeStopDistance;
            
            var player = _blackboard.GetData<Transform>(_stats.PlayerTag);
            var direction = Vec2.Direction(_transform.position, player.position);
            
            if (Vec2.DistanceBetween(_stats, _transform, player, _range))
            {
                //TODO: random idle animations and/or behaviors, like looking around or jumping
                _animator.SetBool("IsWalking", false);
                _audioSource.Stop();
                Vec2.LookAt(_rigid, direction);

                if (_stats.HasBackedUp && !HasEatable())
                {
                    _rigid.velocity = new Vector2(_rigid.velocity.x, 0);
                    _rigid.velocity += (-direction + Vector2.up) * _stats.JumpForce;
                    
                    _stats.HasBackedUp = false;
                }
                
                State = NodeState.Running;
                return State;
            }

            State = NodeState.Failure;
            return State;
        }
        
        private bool HasEatable()
        {
            return _player.GetEatablesCount > 0;
        }
    }
}