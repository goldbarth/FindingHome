using BehaviorTree.Core;
using Player;
using Player.PlayerData;
using UnityEngine;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckIfPlayerHasEatable : ConditionNode
    {
        private readonly IEatables _eatables;
        
        public CheckIfPlayerHasEatable(IEatables eatables)
        {
            _eatables = eatables;
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