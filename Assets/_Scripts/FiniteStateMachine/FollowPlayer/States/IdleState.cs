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
        
        private Vector2 _direction;
        private Transform _target;

        public IdleState(RangeType rangeType, NpcData stats, PlayerController player, Transform transform,
            Animator animator, AudioSource audioSource, IBlackboard blackboard)
        {
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
            _target = GetTarget();
            _direction = GetDirection();

            _animator.SetBool("IsWalking", false);
            _audioSource.Stop();
            Vec2.LookAt(_rigid, _direction);

            if (_stats.HasBackedUp && !HasEatable())
            {
                Backup();
                _stats.HasBackedUp = false;
            }

            StateController.ChangeState(_rangeType == RangeType.Protect
                ? StateController.FriendlyFollowState
                : StateController.SuspiciousFollowState);
        }

        private void Backup()
        {
            _rigid.velocity = new Vector2(_rigid.velocity.x, 0);
            _rigid.velocity += (-_direction + Vector2.up) * _stats.JumpForce;
        }

        private Vector2 GetDirection()
        {
            return Vec2.Direction(_transform.position, _target.position);
        }

        private Transform GetTarget()
        {
            return _blackboard.GetData<Transform>(_stats.PlayerTag);
        }

        private bool HasEatable()
        {
            return _player.GetEatablesCount > 0;
        }
    }
}