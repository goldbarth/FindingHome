using BehaviorTree.Blackboard;
using BehaviorTree.NPCStats;
using BehaviorTree.Core;
using UnityEngine;
using AddIns;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionMoveAwayFromPlayer : ActionNode
    {
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly SpitterStats _stats;
        private readonly Rigidbody2D _rigid;
        private readonly Animator _animator;

        public ActionMoveAwayFromPlayer(SpitterStats stats, Transform transform, Animator animator, IBlackboard blackboard)
        {
            _rigid = transform.GetComponent<Rigidbody2D>();
            _blackboard = blackboard;
            _transform = transform;
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
            var reverseDirection = Vec2.Direction(player.position, position);
            var stopDistance = _stats._detectionRadiusPlayer - _stats._farRangeStopDistance;
            var backup = reverseDirection * _stats._backupDistance;

            // follow phase
            if (distance > stopDistance)
            {
                var step = _stats._speedGoToPlayer * Time.deltaTime;
                _animator.SetBool("IsWalking", true);
                Vec2.MoveTo(_transform, player, step);
                Vec2.LookAt(_rigid, direction);

                State = NodeState.Success;
                return State;
            }
            // backup phase
            if (distance < stopDistance - _stats._backupDistance)
            {
                var step = _stats._speedTargetFollow * Time.deltaTime;
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