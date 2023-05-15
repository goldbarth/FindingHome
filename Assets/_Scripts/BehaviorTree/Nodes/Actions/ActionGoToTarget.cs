using BehaviorTree.Blackboard;
using BehaviorTree.NPCStats;
using BehaviorTree.Core;
using UnityEngine;
using AddIns;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionGoToTarget : ActionNode
    {
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly SpitterStats _stats;
        private readonly Animator _animator;
        private readonly Rigidbody2D _rigid;
        
        private Vector2 _velocity;
        private float _currentSpeed;

        public ActionGoToTarget(SpitterStats stats, Transform transform, Animator animator, IBlackboard blackboard)
        {
            _rigid = transform.parent.GetComponent<Rigidbody2D>();
            _transform = transform.parent;
            _blackboard = blackboard;
            _animator = animator;
            _stats = stats;
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
            var percentage = distance / _stats.TargetStopDistance;
            var speed = Mathf.Lerp(_currentSpeed, _stats.SpeedTargetFollow, percentage);
            var step = _stats.SpeedTargetFollow * Time.deltaTime;
            var targetPosition = Vector2.MoveTowards(_transform.position, target.position, step);
            
            if (distance > _stats.TargetStopDistance)
            {
                _currentSpeed = Mathf.SmoothDamp(_currentSpeed, speed, ref _velocity.x, _stats.SmoothTime);
                _transform.position = Vector2.Lerp(_transform.position, targetPosition, _stats.PositionTransition * Time.deltaTime);
                Vec2.LookAt(_rigid, direction);

                _animator.SetBool("IsWalking", true);
                
                State = NodeState.Running;
                return State; 
            }

            if (distance < _stats.TargetStopDistance)
            {
                _animator.SetBool("IsWalking", false);
                return State = NodeState.Success;
            }
            
            State = NodeState.Failure;
            return State;
        }
    }
}