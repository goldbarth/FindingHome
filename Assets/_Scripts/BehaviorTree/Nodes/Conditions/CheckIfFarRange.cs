using BehaviorTree.Core;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckIfFarRange : ConditionNode
    {
        public bool IsFarRange { get; set; } = false;

        public override NodeState Evaluate()
        {
            IsFarRange = IsFarRange ? IsFarRange = false : IsFarRange = true;
            
            State = NodeState.Success;
            return State;
        }
    }
}