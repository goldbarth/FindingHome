using BehaviorTree.Blackboard;
using BehaviorTree.Core;
using UnityEngine;
using AddIns;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionProtectPlayer : ActionNode
    {
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly Animator _animator;
        private readonly Rigidbody2D _rigid;
        private readonly float _stopDistance;
        private readonly float _speed;
        
        public ActionProtectPlayer(float speed, float stopDistance, Transform transform, IBlackboard blackboard)
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
            var distance = Vector2.Distance(_transform.position, player.position);
            var direction = ((Vector2)player.position - (Vector2)_rigid.transform.position).normalized;
            var step = _speed * Time.deltaTime;
            if (distance > _stopDistance)
            {
                Vec2.MoveTo(_transform, player, step);
                Vec2.LookAt(_rigid, direction);
                _animator.SetBool("IsWalking", true);
            }
            else
            {
                _animator.SetBool("IsWalking", false);
            }

            State = NodeState.Success;
            return State;
        }
    }
}