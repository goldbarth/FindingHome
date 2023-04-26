using Player.PlayerData;
using UnityEngine;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckIfPlayerHasEatable : LeafNode
    {
        private readonly EatablesCount _eatables;
        
        public CheckIfPlayerHasEatable()
        {
            _eatables = GameObject.FindWithTag("Player").GetComponent<EatablesCount>();
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
            return _eatables.GetCount() > 0;
        }
    }
}