using BehaviorTree.Core;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckIfFarRange : ConditionNode
    {
        private bool _isFarRange = false;
        public CheckIfFarRange()
        {
            
        }
        
        public override NodeState Evaluate()
        {
            if (!_isFarRange)
            {
                _isFarRange = true;
                //TODO: logic here
                
                State = NodeState.Success;
                return State;
            }

            State = NodeState.Failure;
            return State;
        }
    }
}