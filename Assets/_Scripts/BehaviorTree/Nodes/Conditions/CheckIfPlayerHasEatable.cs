using BehaviorTree.Core;
using UnityEngine;
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
            if (HasEatable())
            {
                State = NodeState.Success;
                return State;
            }

            State = NodeState.Failure;
            return State;
        }

        private bool HasEatable()
        {
            return _player.GetEatablesCount > 0;
        }
    }
}