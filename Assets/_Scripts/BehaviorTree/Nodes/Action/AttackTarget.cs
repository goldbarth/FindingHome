using BehaviorTree.Behaviors;
using UnityEngine;

namespace BehaviorTree.Nodes.Action
{
    public class AttackTarget : Node
    {
        private SummonerBehavior _summoner;
        public override NodeState Evaluate()
        {
            var target = (Transform)GetData(_summoner.TargetID);
            return NodeState.FAILURE;
        }
    }
}