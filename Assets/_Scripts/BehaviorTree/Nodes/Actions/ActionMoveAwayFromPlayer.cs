using AddIns;
using BehaviorTree.Blackboard;
using BehaviorTree.Core;
using UnityEngine;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionMoveAwayFromPlayer : ActionNode
    {
        private const float BackupDistance = 2f;
        
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly Rigidbody2D _rigid;
        private readonly Animator _animator;
        private readonly float _speedMoveAway;
        private readonly float _stopDistance;
        private readonly float _speedGoTo;
        
        public ActionMoveAwayFromPlayer(float speedGoTo, float speedMoveAway, float detectionRadius, float stopDistance, Transform transform, Animator animator, IBlackboard blackboard)
        {
            _rigid = transform.GetComponent<Rigidbody2D>();
            _stopDistance = detectionRadius - stopDistance;
            _speedMoveAway = speedMoveAway;
            _blackboard = blackboard;
            _speedGoTo = speedGoTo;
            _transform = transform;
            _animator = animator;
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

            // follow phase
            if (distance > _stopDistance)
            {
                var step = _speedGoTo * Time.deltaTime;
                _animator.SetBool("IsWalking", true);
                Vec2.MoveTo(_transform, player, step);
                Vec2.LookAt(_rigid, direction);

                State = NodeState.Success;
                return State;
            }
            // backup phase
            if (distance < _stopDistance - BackupDistance)
            {
                var step = _speedMoveAway * Time.deltaTime;
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