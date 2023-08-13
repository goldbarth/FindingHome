using BehaviorTree.Blackboard;
using BehaviorTree.Core;
using HelpersAndExtensions;
using UnityEngine;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionTryReachTarget : ActionNode
    {
        private readonly IBlackboard _blackboard;
        private readonly Animator _animator;
        private readonly Rigidbody2D _rigid;
        private readonly float _jumpForce;
        private readonly float _interval;

        private float _timer;
        
        public ActionTryReachTarget(float jumpForce, float interval,Animator animator, Transform transform, IBlackboard blackboard)
        {
            _rigid = transform.parent.GetComponent<Rigidbody2D>();
            _blackboard = blackboard;
            _jumpForce = jumpForce;
            _interval = interval;
            _animator = animator;
        }

        public override NodeState Evaluate()
        {
            var target = _blackboard.GetData<Transform>("target");
            var direction = Vec2.Direction(_rigid.transform.position, target.position);
            _timer += Time.deltaTime;
            if (_timer >= _interval)
            {
                _animator.SetBool("IsAttacking", true);
                _rigid.velocity = new Vector2(_rigid.velocity.x, 0);
                _rigid.velocity += direction * _jumpForce;
                _timer = 0;

                State = NodeState.Running;
                return State;
            }

            State = NodeState.Failure;
            return State;
        }
    }
}