using BehaviorTree.Core;
using FiniteStateMachine.SearchAndDestroy.Controller;

namespace BehaviorTree.Nodes.Custom
{
    public class StateMachineNode : BaseNode
    {
        private readonly StateController _stateController;

        public StateMachineNode(StateController stateController)
        {
            _stateController = stateController;
        }

        public override NodeState Evaluate()
        {
            _stateController.ChangeState(_stateController.ChaseState);
            
            return NodeState.Failure;
        }
    }
}