using BehaviorTree.Core;
using Player;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckIfPlayerHasEatable : ConditionNode
    {
        private readonly PlayerController _player;

        public CheckIfPlayerHasEatable(PlayerController player)
        {
            _player = player;
        }
        
        public override NodeState Evaluate()
        {
            return State = HasEdible() ? NodeState.Success : NodeState.Failure;
        }

        private bool HasEdible()
        {
            return _player.GetEatablesCount > 0;
        }
    }
}