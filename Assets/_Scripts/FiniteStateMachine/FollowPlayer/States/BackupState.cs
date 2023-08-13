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

        private Vector2 _velocity;

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
            var player = _blackboard.GetData<Transform>(_stats.PlayerTag);
            var position = _transform.position;
            var reverseDirection = Vec2.Direction(player.position, position);
            var backup = reverseDirection * _stats.MaxBackupDistance;

            _transform.position = Vector2.SmoothDamp(position, position + (Vector3)backup, ref _velocity,
                _stats.SmoothTimeBackup);
            Vec2.LookAt(_rigid, reverseDirection);

            _animator.SetBool("IsWalking", true);
            _audioSource.PlayOneShot(_audioSource.clip);

            _stats.HasBackedUp = true;
        }
    }
}