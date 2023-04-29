using BehaviorTree.Core;
using UnityEngine;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckIfInAttackPhase : ConditionNode
    {
        public CheckIfInAttackPhase() : base() { }

        public override NodeState Evaluate()
        {
            if (GameManager.Instance.IsInAttackPhase)
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