using BehaviorTree.Blackboard;
using BehaviorTree.NPCStats;
using BehaviorTree.Core;
using UnityEngine;
using AddIns;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionBackupPlayer : ActionNode
    {
        private readonly AudioSource _audioSource;
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly SpitterStats _stats;
        private readonly Rigidbody2D _rigid;
        private readonly Animator _animator;
        
        private Vector2 _velocity;

        public ActionBackupPlayer(SpitterStats stats, Transform transform, Animator animator, AudioSource audioSource, IBlackboard blackboard)
        {
            _rigid = transform.parent.GetComponent<Rigidbody2D>();
            _transform = transform.parent;
            _audioSource = audioSource;
            _blackboard = blackboard;
            _animator = animator;
            _stats = stats;
        }
        
        public override NodeState Evaluate()
        {
            var player = _blackboard.GetData<Transform>(_stats.PlayerTag);
            var position = _transform.position;
            var reverseDirection = Vec2.Direction(player.position, position);
            var backup = reverseDirection * _stats.MaxBackupDistance;

            if (Vec2.BackupDistance(_stats, _transform, player))
            {
                _transform.position = Vector2.SmoothDamp(position, position + (Vector3)backup, ref _velocity, _stats.SmoothTimeBackup);
                Vec2.LookAt(_rigid, reverseDirection);

                _animator.SetBool("IsWalking", true);
                _audioSource.PlayOneShot(_audioSource.clip);
                
                _stats.HasBackedUp = true;
                State = NodeState.Running;
                return State;
            }
            
            _velocity = Vector2.zero;

            State = NodeState.Failure;
            return State;
        }
    }
}