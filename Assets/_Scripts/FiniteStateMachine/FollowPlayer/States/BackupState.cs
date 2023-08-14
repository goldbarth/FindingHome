using BehaviorTree.Blackboard;
using FiniteStateMachine.Base;
using HelpersAndExtensions;
using NpcSettings;
using UnityEngine;

namespace FiniteStateMachine.FollowPlayer.States
{
    public class BackupState : State
    {
        private readonly AudioSource _audioSource;
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly Rigidbody2D _rigid;
        private readonly Animator _animator;
        private readonly NpcData _stats;

        private Vector2 _direction;
        private Vector3 _position;
        private Vector2 _velocity;
        private Transform _target;

        public BackupState(NpcData stats, Transform transform, Animator animator, AudioSource audioSource,
            IBlackboard blackboard)
        {
            _rigid = transform.parent.GetComponent<Rigidbody2D>();
            _transform = transform.parent;
            _audioSource = audioSource;
            _blackboard = blackboard;
            _animator = animator;
            _stats = stats;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            BackupPhase();
        }

        private void BackupPhase()
        {
            _target = GetTarget();
            _position = _transform.position;
            _direction = GetDirection();
            
            var backupDistance = CalculateBackupDistance();
            _transform.position = CalculateSmoothDamp(backupDistance);
            
            Vec2.LookAt(_rigid, _direction);

            _animator.SetBool("IsWalking", true);
            _audioSource.PlayOneShot(_audioSource.clip);

            _stats.HasBackedUp = true;
        }

        private Vector2 GetDirection()
        {
            return Vec2.Direction(_target.position, _position);
        }

        private Vector2 CalculateSmoothDamp(Vector2 backup)
        {
            return Vector2.SmoothDamp(_position, _position + (Vector3)backup, ref _velocity,_stats.SmoothTimeBackup);
        }

        private Vector2 CalculateBackupDistance()
        {
            return _direction * _stats.MaxBackupDistance;
        }

        private Transform GetTarget()
        {
            return _blackboard.GetData<Transform>(_stats.PlayerTag);
        }
    }
}