using BehaviorTree.Core;
using BehaviorTree.NPCStats;
using UnityEngine;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckIfInAttackPhase : ConditionNode
    {
        private readonly SpitterStats _stats;

        public CheckIfInAttackPhase(SpitterStats stats)
        {
            _stats = stats;
        }
        
        public override NodeState Evaluate()
        {
            if (_stats._isInAttackPhase)
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