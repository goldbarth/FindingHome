using BehaviorTree.Blackboard;
using BehaviorTree.Core;
using UnityEngine;
using AddIns;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionFollowAndBackupToPlayer : ActionNode
    {
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly Rigidbody2D _rigid;
        private readonly Animator _animator;
        private readonly float _backupDistance;
        private readonly float _stopDistance;
        private readonly float _speed;

        public ActionFollowAndBackupToPlayer(float speed, float detectionRadius, float stopDistance, float backupDistance, Transform transform, Animator animator, IBlackboard blackboard)
        {
            _animator = transform.parent.GetComponentInChildren<Animator>();
            _rigid = transform.parent.GetComponent<Rigidbody2D>();
            _stopDistance = detectionRadius - stopDistance;
            _backupDistance = backupDistance;
            _blackboard = blackboard;
            _transform = transform.parent;
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
            var backup = reverseDirection * _backupDistance;
            var step = _speed * Time.deltaTime;
            
            DrawLineSegments(position, player.position, _stopDistance);

            // idle phase
            if (distance < _stopDistance - .01f && distance > _stopDistance - _backupDistance + .01f)
            {
                Debug.Log("idle");
                _animator.SetBool("IsWalking", false);
                Vec2.LookAt(_rigid, direction);
                
                State = NodeState.Running;
                return State;
            }
            
            // follow phase
            if (distance > _stopDistance)
            {
                _animator.SetBool("IsWalking", true);
                Vec2.MoveTo(_transform, player, step);
                Vec2.LookAt(_rigid, direction);

                State = NodeState.Success;
                return State;
            }
            // backup phase
            if (distance < _stopDistance - _backupDistance)
            {
                _animator.SetBool("IsWalking", true);
                Vec2.MoveAway(_transform, position, backup, step);
                Vec2.LookAt(_rigid, reverseDirection);

                State = NodeState.Success;
                return State;
            }

            State = NodeState.Failure;
            return State;
        }
        
        private void DrawLineSegments(Vector2 position, Vector2 player, float distance)
        {
            var dir = (player - position);
            var stopDir = dir.magnitude;
            var shortenedDirection = dir.normalized * (stopDir - distance);
            var backupShortenedDirection = dir.normalized * (stopDir - distance + _backupDistance);
            var endPoint = position + shortenedDirection;
            var backupEndPoint = position + backupShortenedDirection;
            
            Debug.DrawLine(position, endPoint, Color.green);
            Debug.DrawLine(endPoint, backupEndPoint, Color.red);
            Debug.DrawLine(backupEndPoint, player, Color.magenta);
        }
    }
}