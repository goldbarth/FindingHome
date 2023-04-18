using Player.PlayerData;
using UnityEngine;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckPlayerHasEatable : LeafNode
    {
        private readonly EatablesCount _eatables;
        
        public CheckPlayerHasEatable()
        {
            _eatables = GameObject.FindWithTag("Player").GetComponent<EatablesCount>();
        }
        
        public override NodeState Evaluate()
        {
            if (HasEatable())
            {
                Debug.Log("Player has eatable");
                State = NodeState.SUCCESS;
                return State;
            }

            State = NodeState.FAILURE;
            return State;
        }

        private bool HasEatable()
        {
            return _eatables.GetCount() > 0;
        }
    }
}