using BehaviorTree.Blackboard;
using FiniteStateMachine.Base;
using HelpersAndExtensions;
using NpcSettings;
using UnityEngine;
using System;

namespace FiniteStateMachine.FollowPlayer.States
{
    public class FollowState : State
    {
        private readonly AudioSource _audioSource;
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly RangeType _rangeType;
        private readonly Rigidbody2D _rigid;
        private readonly Animator _animator;
        private readonly NpcData _stats;

        private readonly float _smoothTime;
        
        private Vector2 _velocity;
        private float _range;

        public FollowState(RangeType rangeType, NpcData stats, Transform transform, Animator animator, AudioSource audioSource, IBlackboard blackboard)
        {
            switch (rangeType)
            {
                case RangeType.Near:
                    _range = stats.NearRangeStopDistance;
                    _smoothTime = stats.SmoothTime;
                    break;
                case RangeType.Far:
                    _range = stats.FarRangeStopDistance;
                    _smoothTime = stats.SmoothTime;
                    break;
                case RangeType.Protect:
                    _range = stats.ProtectRangeStopDistance;
                    _smoothTime = stats.SmoothTimeFast;
                    break;
            }
            
            _rigid = transform.parent.GetComponent<Rigidbody2D>();
            _transform = transform.parent;
            _audioSource = audioSource;
            _blackboard = blackboard;
            _rangeType = rangeType;
            _animator = animator;
            _stats = stats;
        }
        
        protected override void OnUpdate()
        {
            base.OnUpdate();
            FollowPhase();
        }
        
        private void FollowPhase()
        {
            if (!_blackboard.ContainsKey(_stats.PlayerTag))
                return;
            
            ChangeDistanceWhenHasEaten();
            
            var player = SetPlayerData();
            if (player == null) return;
            
            
            if (IsDistanceGreaterThanStopDistance(player, _transform.position))
                MoveToPlayer(player);
            else
            {
                _velocity = Vector2.zero;
                StateController.ChangeState(_rangeType == RangeType.Protect
                    ? StateController.FriendlyIdleState
                    : StateController.SuspiciousIdleState);
            }
        }

        private void MoveToPlayer(Transform player)
        {
            var position = _transform.position;
            var direction = Vec2.Direction(position, player.position);
            
            _transform.position = Vector2.SmoothDamp(position, player.position, ref _velocity, _smoothTime);
            Vec2.LookAt(_rigid, direction);

            _animator.SetBool("IsWalking", true);
            _audioSource.PlayOneShot(_audioSource.clip);

            _stats.HasBackedUp = false;
        }

        private void ChangeDistanceWhenHasEaten()
        {
            if (_stats.HasEaten)
                _range = _stats.IsFarRange ? _stats.FarRangeStopDistance : _stats.ProtectRangeStopDistance;
        }

        private Transform SetPlayerData()
        {
            return _blackboard.GetData<Transform>(_stats.PlayerTag);
        }
        
        private bool IsDistanceGreaterThanStopDistance(Transform player, Vector2 position)
        {
            var distance = Vector2.Distance(position, player.position);
            var stopDistance = _stats.DetectionRadiusPlayer - _range;
            return distance > stopDistance;
        }
    }
}