using UnityEngine.InputSystem;
using BehaviorTree.Core;
using UnityEngine;

namespace BehaviorTree.Nodes.Conditions
{
    public class CheckIfPlayerWasCommanding : ConditionNode
    {
        private readonly Controls _controls;
        private readonly InputAction _commandAction;
        private bool _isFarRange = false;

        public CheckIfPlayerWasCommanding()
        {
            _controls = new Controls();
            _commandAction = _controls.Gameplay.Interact;
        }

        public override NodeState Evaluate()
        {
            Debug.Log("CheckIfPlayerWasCommanding: " + _commandAction.triggered + "Is Far: " + _isFarRange);
            if (_commandAction.triggered && !_isFarRange)
            {
                // command
                _isFarRange = true;
                Debug.Log("command");
                
                State = NodeState.Success;
                return State;
            }
            if (_commandAction.triggered && _isFarRange)
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
    }
}