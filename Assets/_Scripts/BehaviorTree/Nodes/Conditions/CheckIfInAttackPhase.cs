using BehaviorTree.Core;
using BehaviorTree.Nodes.Actions;
using UnityEngine;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckIfInAttackPhase : ConditionNode
    {
        public CheckIfInAttackPhase() : base() { }

        public override NodeState Evaluate()
        {
            if (CheckIfTargetInAttackRange.IsInAttackRange)
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