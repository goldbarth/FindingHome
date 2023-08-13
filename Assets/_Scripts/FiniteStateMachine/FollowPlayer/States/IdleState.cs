using BehaviorTree.Blackboard;
using FiniteStateMachine.Base;
using HelpersAndExtensions;
using NpcSettings;
using UnityEngine;
using Player;

namespace FiniteStateMachine.FollowPlayer.States
{
    public class IdleState : State
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

        public IdleState(RangeType rangeType, NpcData stats, PlayerController player, Transform transform,
            Animator animator, AudioSource audioSource, IBlackboard blackboard)
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
            _rangeType = rangeType;
            _animator = animator;
            _player = player;
            _stats = stats;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            IdlePhase();
        }

        private void IdlePhase()
        {
            var player = _blackboard.GetData<Transform>(_stats.PlayerTag);
            var direction = Vec2.Direction(_transform.position, player.position);

            _animator.SetBool("IsWalking", false);
            _audioSource.Stop();
            Vec2.LookAt(_rigid, direction);

            if (_stats.HasBackedUp && !HasEatable())
            {
                _rigid.velocity = new Vector2(_rigid.velocity.x, 0);
                _rigid.velocity += (-direction + Vector2.up) * _stats.JumpForce;

                _stats.HasBackedUp = false;
            }

            StateController.ChangeState(_rangeType == RangeType.Protect
                ? StateController.FriendlyFollowState
                : StateController.SuspiciousFollowState);
        }

        private bool HasEatable()
        {
            return _player.GetEatablesCount > 0;
        }
    }
}