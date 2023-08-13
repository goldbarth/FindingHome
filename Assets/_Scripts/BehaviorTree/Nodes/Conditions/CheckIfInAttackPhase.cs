using BehaviorTree.Core;
using NpcSettings;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckIfInAttackPhase : ConditionNode
    {
        private readonly NpcData _stats;

        public CheckIfInAttackPhase(NpcData stats)
        {
            _stats = stats;
        }
        
        public override NodeState Evaluate()
        {
            return State = _stats.IsInAttackPhase ? NodeState.Success : NodeState.Failure;
        }
    }
}