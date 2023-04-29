using BehaviorTree.Core;
using Player;
using Player.PlayerData;
using UnityEngine;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckIfFriendlyNPCHasEaten : ConditionNode
    {
        private readonly IEatables _eatables;
        
        private bool _hasEaten = false;

        public CheckIfFriendlyNPCHasEaten(IEatables eatables)
        {
            _eatables = eatables;
        }

        public override NodeState Evaluate()
        {
            if (IsFull() && !_hasEaten)
            {
                Debug.Log("Target has eaten");
                _hasEaten = true;
                State = NodeState.Success;
                return State;
            }
            if(_hasEaten)
            {
                Debug.Log("Target is still full");
                State = NodeState.Success;
                return State;
            }

            State = NodeState.Failure;
            return State;
        }
        
        private bool IsFull()
        {
            return _eatables.HasEatableDecreased();
        }
    }
}