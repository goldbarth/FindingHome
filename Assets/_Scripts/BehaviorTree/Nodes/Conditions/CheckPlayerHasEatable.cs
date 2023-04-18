using DataPersistence;
using UnityEngine;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckPlayerHasEatable : LeafNode
    {
        private GameData _gameData;
        
        public CheckPlayerHasEatable()
        {
            _gameData = new GameData();
        }

        public override NodeState Evaluate()
        {
            if (HasEatable(_gameData))
            {
                Debug.Log("Player has eatable");
                State = NodeState.SUCCESS;
                return State;
            }


            State = NodeState.FAILURE;
            return State;
        }

        private bool HasEatable(GameData gameData)
        {
            return gameData.HasEatable();
        }
    }
}