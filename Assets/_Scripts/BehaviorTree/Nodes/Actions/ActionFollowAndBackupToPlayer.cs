using BehaviorTree.Blackboard;
using BehaviorTree.Core;
using UnityEngine;
using AddIns;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionFollowAndBackupToPlayer : ActionNode
    {
        private const float BackupDistance = 0.5f;
        
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly Rigidbody2D _rigid;
        private readonly Animator _animator;
        private readonly float _stopDistance;
        private readonly float _speed;

        public ActionFollowAndBackupToPlayer(float speed, float stopDistance, Transform transform, IBlackboard blackboard)
        {
            _animator = transform.GetComponentInChildren<Animator>();
            _rigid = transform.GetComponent<Rigidbody2D>();
            _stopDistance = stopDistance;
            _blackboard = blackboard;
            _transform = transform;
            _speed = speed;
        }
        
        public override NodeState Evaluate()
        {
            if (GameManager.Instance.IsInAttackPhase)
            {
                State = NodeState.Failure;
                return State;
            }
            
            var player = _blackboard.GetData<Transform>("player");
            var position = _transform.position;
            var distance = Vector2.Distance(position, player.position);
            var direction = Vec2.Direction(position, player.position);
            var reverseDirection = Vec2.Direction(player.position, position);
            var backup = reverseDirection * BackupDistance;
            var step = _speed * Time.deltaTime;
            if (distance > _stopDistance)
            {
                _animator.SetBool("IsWalking", true);
                Vec2.MoveTo(_transform, player, step);
                Vec2.LookAt(_rigid, direction);

                State = NodeState.Success;
                return State;
            }
            if (distance < _stopDistance - BackupDistance)
            {
                _animator.SetBool("IsWalking", true);
                Vec2.MoveAway(_transform, position, backup, step);
                Vec2.LookAt(_rigid, reverseDirection);

                State = NodeState.Success;
                return State;
            }
            
            _animator.SetBool("IsWalking", false);

            State = NodeState.Failure;
            return State;
        }
    }
}