using Player.PlayerData;
using UnityEngine;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckIfFriendlyNPCHasEaten : LeafNode
    {
        private readonly EatablesCount _eatables;
        private bool _hasEaten = false;

        public CheckIfFriendlyNPCHasEaten()
        {
            _eatables = GameObject.FindWithTag("Player").GetComponent<EatablesCount>();
        }
        
        
        public override NodeState Evaluate()
        {
            if (_eatables.HasEatableDecreased() && !_hasEaten)
            {
                Debug.Log("Target has eaten");
                _hasEaten = true;
                State = NodeState.Success;
                return State;
            }
            if(_hasEaten)
            {
                // TODO: Check if friendly follower died, if so, set state to failure.
                Debug.Log("Target is still full");
                State = NodeState.Success;
                return State;
            }

            State = NodeState.Failure;
            return State;
        }
    }
}