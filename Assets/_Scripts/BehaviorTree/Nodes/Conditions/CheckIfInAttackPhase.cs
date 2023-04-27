using BehaviorTree.Nodes.Actions;
using UnityEngine;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckIfInAttackPhase : LeafNode
    {
        public CheckIfInAttackPhase() : base()
        { }

        public override NodeState Evaluate()
        {

            if (ActionAttackTarget.IsInAttackPhase)
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