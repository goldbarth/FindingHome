using UnityEngine;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionIdle : LeafNode
    {
        private Animator _animator;
        
        public ActionIdle(Transform transform)
        {
            _animator = transform.GetComponentInChildren<Animator>();
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