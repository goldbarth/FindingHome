using BehaviorTree.Nodes.Actions;
using UnityEngine;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckIfInAttackPhase : LeafNode
    {
        private readonly ActionAttackTarget _attackTarget;

        public CheckIfInAttackPhase()
        {
            _attackTarget = new ActionAttackTarget();
        }
        
        public override NodeState Evaluate()
        {
            if (_attackTarget.IsAttacking)
            {
                Debug.Log("In attack phase");
                State = NodeState.Success;
                return State;
            }
            
            State = NodeState.Failure;
            return State;
        }
    }
}