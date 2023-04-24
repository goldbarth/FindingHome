using Player.PlayerData;
using UnityEngine;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckTargetHasEaten : Node
    {
        //private readonly EatablesCount _eatables;
        private bool _hasEaten = false;
        
        public CheckTargetHasEaten(bool hasEaten)
        {
            //_eatables = GameObject.FindWithTag("Player").GetComponent<EatablesCount>();
            _hasEaten = hasEaten;
        }
        
        public override NodeState Evaluate()
        {
            if (_hasEaten)
            {
                Debug.Log("Target has eaten");
                State = NodeState.Success;
                return State;
            }

            State = NodeState.Failure;
            return State;
        }
    }
}