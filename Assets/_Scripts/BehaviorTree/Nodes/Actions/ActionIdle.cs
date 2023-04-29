using BehaviorTree.Core;
using UnityEngine;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionIdle : ActionNode
    {
        private Animator _animator;
        
        public ActionIdle(Component component)
        {
            _animator = component.GetComponentInChildren<Animator>();
        }

        public override NodeState Evaluate()
        {
            //_animator.SetBool("IsWalking", false);
            //_animator.SetBool("IsAttacking", false);
            
            State = NodeState.Running;
            return State;
        }
    }
}