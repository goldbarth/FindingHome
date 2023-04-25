using Player.PlayerData;
using UnityEngine;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckHasEaten : LeafNode
    {
        private readonly EatablesCount _eatables;
        private readonly bool _hasEaten;
        
        //public CheckHasEaten(bool hasEaten)
        //{
        //    _eatables = GameObject.FindWithTag("Player").GetComponent<EatablesCount>();
        //    _hasEaten = hasEaten;
        //}
        
        
        public override NodeState Evaluate()
        {
            if (true)
            {
                Debug.Log("Target has eaten");
                State = NodeState.Success;
                return State;
            }

            State = NodeState.Failure;
            return State;
        }
    }

    public class CopyOfCheckHasEaten : LeafNode
    {
        private readonly EatablesCount _eatables;
        private readonly bool _hasEaten;

        //public CopyOfCheckHasEaten(bool hasEaten)
        //{
        //    _eatables = GameObject.FindWithTag("Player").GetComponent<EatablesCount>();
        //    _hasEaten = hasEaten;
        //}


        public override NodeState Evaluate()
        {
            if (true)
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