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
            return State = _stats.IsInAttackPhase ? NodeState.Success : NodeState.Failure;
        }
    }
}