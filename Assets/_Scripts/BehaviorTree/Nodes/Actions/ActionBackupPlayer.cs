using BehaviorTree.Blackboard;
using BehaviorTree.NPCStats;
using BehaviorTree.Core;
using UnityEngine;
using AddIns;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionBackupPlayer : ActionNode
    {
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly SpitterStats _stats;
        private readonly Rigidbody2D _rigid;
        private readonly Animator _animator;
        
        private Vector2 _velocity;

        public ActionBackupPlayer(SpitterStats stats, Transform transform, Animator animator, IBlackboard blackboard)
        {
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
            
            var player = _blackboard.GetData<Transform>(_stats.PlayerTag);
            var position = _transform.position;
            var distance = Vector2.Distance(position, player.position);
            var stopDistance = _stats.DetectionRadiusPlayer - _stats.NearRangeStopDistance;
            var reverseDirection = Vec2.Direction(player.position, position);
            var backup = reverseDirection * _stats.MaxBackupDistance;

            if (distance < stopDistance - _stats.MinBackupDistance && distance < stopDistance - _stats.MaxBackupDistance)
            {
                _transform.position = Vector2.SmoothDamp(position, position + (Vector3)backup, ref _velocity, _stats.SmoothTimeFast);
                Vec2.LookAt(_rigid, reverseDirection);

                _animator.SetBool("IsWalking", true);
                
                Debug.Log("Backup");
                State = NodeState.Running;
                return State;
            }
            
            _velocity = Vector2.zero;

            State = NodeState.Failure;
            return State;
        }
    }
}