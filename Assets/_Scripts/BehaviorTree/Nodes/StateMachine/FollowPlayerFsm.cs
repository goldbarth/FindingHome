using FiniteStateMachine.Controller;
using FiniteStateMachine.Base;
using BehaviorTree.Core;

namespace BehaviorTree.Nodes.StateMachine
{
    public class FollowPlayerFsm : StateMachineNode
    {
        private readonly StateController _stateController;
        private readonly Transition _isInIdleRange;
        private readonly RangeType _rangeType;
        private readonly State _followState;
        private readonly State _idleState;

        public FollowPlayerFsm(StateController stateController, RangeType rangeType) : base(stateController)
        {
            _stateController = stateController;
            _rangeType = rangeType;
            
            switch (rangeType)
            {
                case RangeType.Protect:
                    _isInIdleRange = _stateController.IsInFriendlyIdleRange;
                    _followState = _stateController.FriendlyFollowState;
                    _idleState = _stateController.FriendlyIdleState;
                    break;
                case RangeType.Near:
                    _isInIdleRange = _stateController.IsInSuspiciousIdleRange;
                    _followState = _stateController.SuspiciousFollowState;
                    _idleState = _stateController.SuspiciousIdleState;
                    break;
            }
        }
        
        public override NodeState Evaluate()
        {
            if (_stateController.IsPlayerInFOV.OnCanTransitionTo())
                _stateController.ChangeState(_followState);
            if (_stateController.IsInBackupRange.OnCanTransitionTo() && _rangeType == RangeType.Near)
                _stateController.ChangeState(_stateController.BackupState);
            if(_isInIdleRange.OnCanTransitionTo())
                _stateController.ChangeState(_idleState);
            
            return NodeState.Failure;
        }
    }
}