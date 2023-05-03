using BehaviorTree.Core;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckIfPlayerWasCommanding : ConditionNode
    {
        private Controls _controls;
        private InputAction _commandAction;
        private bool _isFarRange = false;
        private bool _isCommanding;
        
        public CheckIfPlayerWasCommanding()
        {
            _controls = new Controls();
            _commandAction = _controls.Gameplay.Interact;
        }

        public override NodeState Evaluate()
        {
            if (IsCommanding() && !IsFarRange())
            {
                // command
                _isFarRange = true;
                Debug.Log("command");
                
                State = NodeState.Success;
                return State;
            }
            if (IsCommanding() && IsFarRange())
            {
                // backup
                _isFarRange = false;
                Debug.Log("backup");
                
                State = NodeState.Failure;
                return State;
            }

            State = NodeState.Failure;
            return State;
        }

        private bool IsFarRange()
        {
            return _isFarRange;
        }
        
        private bool IsCommanding()
        {
            return _commandAction.triggered;
        }
    }
}