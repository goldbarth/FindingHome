using BehaviorTree.Blackboard;
using BehaviorTree.NPCStats;
using BehaviorTree.Core;
using UnityEngine;
using AddIns;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionFollowAndBackupToPlayer : ActionNode
    {
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly SpitterStats _stats;
        private readonly Rigidbody2D _rigid;
        private readonly Animator _animator;

        public ActionFollowAndBackupToPlayer(SpitterStats stats, Transform transform, Animator animator, IBlackboard blackboard)
        {
            _rigid = transform.parent.GetComponent<Rigidbody2D>();
            _transform = transform.parent;
            _blackboard = blackboard;
            _animator = animator;
            _stats = stats;
        }
        
        public override NodeState Evaluate()
        {
            if (_stats._isInAttackPhase)
            {
                State = NodeState.Failure;
                return State;
            }
            
            var player = _blackboard.GetData<Transform>("player");
            var position = _transform.position;
            var distance = Vector2.Distance(position, player.position);
            var direction = Vec2.Direction(position, player.position);
            var stopDistance = _stats._detectionRadiusPlayer - _stats._nearRangeStopDistance;
            
            var reverseDirection = Vec2.Direction(player.position, position);
            var backup = reverseDirection * _stats._backupDistance;
            var step = _stats._speedGoToPlayer * Time.deltaTime;
            
            DrawLineSegments(position, player.position, stopDistance);

            // idle phase
            if (Vec2.DistanceBetween(_stats, position, player.position))
            {
                _animator.SetBool("IsWalking", false);
                Vec2.LookAt(_rigid, direction);
                
                State = NodeState.Running;
                return State;
            }
            
            // follow phase
            if (distance > stopDistance)
            {
                _animator.SetBool("IsWalking", true);
                Vec2.MoveTo(_transform, player, step);
                Vec2.LookAt(_rigid, direction);

                State = NodeState.Success;
                return State;
            }
            // backup phase
            if (distance < stopDistance - _stats._backupDistance)
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
            var backupShortenedDirection = dir.normalized * (stopDir - distance + _stats._backupDistance);
            var endPoint = position + shortenedDirection;
            var backupEndPoint = position + backupShortenedDirection;
            
            Debug.DrawLine(position, endPoint, Color.green);
            Debug.DrawLine(endPoint, backupEndPoint, Color.red);
            Debug.DrawLine(backupEndPoint, player, Color.magenta);
        }
    }
}