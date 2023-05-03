using BehaviorTree.Core;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckIfFarRange : ConditionNode
    {
        public CheckIfFarRange()
        {
            
        }
        
        public override NodeState Evaluate()
        {
            if (GameManager.Instance.IsFarRange)
            {
                State = NodeState.Success;
                return State;
            }

            State = NodeState.Failure;
            return State;
        }
    }
}