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
            var step = _stats.SpeedTargetFollow * Time.deltaTime;
            if (distance > _stats.TargetStopDistance)
            {
                _animator.SetBool("IsWalking", true);
                Vec2.MoveTo(_transform, target, step);
                Vec2.LookAt(_rigid, direction);

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