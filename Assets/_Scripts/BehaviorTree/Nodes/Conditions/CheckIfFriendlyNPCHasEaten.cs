using BehaviorTree.Core;
using BehaviorTree.NPCStats;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckIfFriendlyNPCHasEaten : ConditionNode
    {
        private readonly SpitterStats _stats;

        public CheckIfFriendlyNPCHasEaten(SpitterStats stats)
        {
            _stats = stats;
        }

        public override NodeState Evaluate()
        {
            return State = _stats._hasEaten ? NodeState.Success : NodeState.Failure;
        }
    }
}