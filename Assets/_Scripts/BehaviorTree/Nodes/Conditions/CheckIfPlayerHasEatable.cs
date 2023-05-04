using Player.PlayerData;
using BehaviorTree.Core;
using Player;
using UnityEngine;

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
                Debug.Log("Player has eatable");
                State = NodeState.Running;
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