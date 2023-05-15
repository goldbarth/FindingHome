using BehaviorTree.NPCStats;
using BehaviorTree.Core;
using Player;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckIfPlayerWasCommanding : ConditionNode
    {
        private readonly PlayerController _player;
        private readonly SpitterStats _stats;

        public CheckIfPlayerWasCommanding(SpitterStats stats, PlayerController player)
        {
            _player = player;
            _stats = stats;
        }

        public override NodeState Evaluate()
        {
            _stats.IsFarRange = _player.IsInteracting && !_stats.IsFarRange ? _stats.IsFarRange = true : _stats.IsFarRange = false;
            return State = _player.IsInteracting ? NodeState.Success : NodeState.Failure;
            // State = NodeState.Success;
            // return State;
        }
    }
}