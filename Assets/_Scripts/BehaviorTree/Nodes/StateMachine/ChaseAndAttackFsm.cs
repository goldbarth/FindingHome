using FiniteStateMachine.Controller;
using BehaviorTree.Core;

namespace BehaviorTree.Nodes.StateMachine
{
    public class ChaseAndAttackFsm : StateMachineNode
    {
        private readonly StateController _stateController;

        public ChaseAndAttackFsm(StateController stateController) : base(stateController)
        {
            _stateController = stateController;
        }
        
        public override NodeState Evaluate()
        {
            if(_stateController.IsEnemyInFOV.OnStateCanTransitionTo())
                _stateController.ChangeState(_stateController.ChaseState);
            if (_stateController.IsInAttackRange.OnStateCanTransitionTo())
                _stateController.ChangeState(_stateController.AttackState);
            
            return NodeState.Success;
        }
    }
}