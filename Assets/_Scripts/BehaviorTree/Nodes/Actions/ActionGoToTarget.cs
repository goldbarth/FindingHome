using BehaviorTree.Blackboard;
using BehaviorTree.Core;
using UnityEngine;
using AddIns;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionGoToTarget : ActionNode
    {
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly Animator _animator;
        private readonly Rigidbody2D _rigid;
        private readonly float _stopDistance;
        private readonly float _speed;
        
        public ActionGoToTarget(float speed, float stopDistance, Transform transform, Animator animator, IBlackboard blackboard)
        {
            _animator = transform.parent.GetComponentInChildren<Animator>();
            _rigid = transform.parent.GetComponent<Rigidbody2D>();
            _transform = transform.parent;
            _stopDistance = stopDistance;
            _blackboard = blackboard;
            _speed = speed;
        }
        
        public override NodeState Evaluate()
        {
            if(!_blackboard.ContainsKey("target"))
            {
                State = NodeState.Failure;
                return State;
            }
            var target = _blackboard.GetData<Transform>("target");
            var direction = Vec2.Direction(_transform.position, target.position);
            var distance = Vector2.Distance(_transform.position, target.position);
            var step = _speed * Time.deltaTime;
            if (distance > _stopDistance)
            {
                _animator.SetBool("IsWalking", true);
                Vec2.MoveTo(_transform, target, step);
                Vec2.LookAt(_rigid, direction);

                State = NodeState.Success;
                return State; 
            }
            
            State = NodeState.Failure;
            return State;
        }
    }
}