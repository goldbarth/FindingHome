using BehaviorTree.Core;
using NpcSettings;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckIfFriendlyNPCHasEaten : ConditionNode
    {
        private readonly NpcData _stats;

        public CheckIfFriendlyNPCHasEaten(NpcData stats)
        {
            _stats = stats;
        }

        public override NodeState Evaluate()
        {
            return State = _stats.HasEaten ? NodeState.Success : NodeState.Failure;
        }
    }
}