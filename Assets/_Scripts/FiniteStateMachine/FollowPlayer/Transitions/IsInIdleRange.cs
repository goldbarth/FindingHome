using BehaviorTree.Blackboard;
using FiniteStateMachine.Base;
using HelpersAndExtensions;
using NpcSettings;
using UnityEngine;
using Player;

namespace FiniteStateMachine.FollowPlayer.Transitions
{
    public class IsInIdleRange : Transition
    {
        private readonly PlayerController _player;
        private readonly AudioSource _audioSource;
        private readonly IBlackboard _blackboard;
        private readonly RangeType _rangeType;
        private readonly Transform _transform;
        private readonly Rigidbody2D _rigid;
        private readonly Animator _animator;
        private readonly NpcData _stats;
        
        private readonly float _range;

        public IsInIdleRange(RangeType rangeType, NpcData stats, Transform transform,   IBlackboard blackboard) : base(stats, transform)
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
            
            _transform = transform.parent;
            _blackboard = blackboard;
            _stats = stats;
        }

        public override bool OnCanTransitionTo()
        {
            var player = _blackboard.GetData<Transform>(_stats.PlayerTag);
            if(player == null) return false;
            return Vec2.DistanceBetween(_stats, _transform, player, _range);
        }
    }
}