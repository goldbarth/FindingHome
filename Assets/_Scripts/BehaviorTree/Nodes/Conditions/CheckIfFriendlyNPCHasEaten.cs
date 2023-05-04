using Player.PlayerData;
using BehaviorTree.Core;
using Player;
using UnityEngine;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckIfFriendlyNPCHasEaten : ConditionNode
    {
        private readonly PlayerController _player;

        private bool _hasEaten = false;

        public CheckIfFriendlyNPCHasEaten(PlayerController player)
        {
            _player = player;
            //_eatables = eatables;
        }

        public override NodeState Evaluate()
        {
            if (_player.HasEatablesDecreased && !_hasEaten)
            {
                Debug.Log("Friendly-NPC has eaten");
                _hasEaten = true;
                State = NodeState.Success;
                return State;
            }
            if(_hasEaten)
            {
                Debug.Log("Friendly-NPC is still full");
                State = NodeState.Success;
                return State;
            }

            State = NodeState.Failure;
            return State;
        }
    }
}