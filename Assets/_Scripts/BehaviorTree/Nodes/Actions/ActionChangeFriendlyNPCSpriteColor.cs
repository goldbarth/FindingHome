using BehaviorTree.Core;
using UnityEditor.Animations;
using UnityEngine;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionChangeFriendlyNPCSpriteColor : ActionNode
    {
        private readonly AnimatorController _newController;
        private readonly Animator _animator;

        private bool _hasChanged;

        public ActionChangeFriendlyNPCSpriteColor(Component component, AnimatorController newController)
        {
            _animator = component.GetComponentInChildren<Animator>();
            _newController = newController;
        }

        public override NodeState Evaluate()
        {
            if (!_hasChanged)
            {
                _animator.runtimeAnimatorController = _newController;
                _animator.SetTrigger("IsEatingTrigger");
                Debug.Log("Change Color");
                _hasChanged = true;
                
                State = NodeState.Success;
                return State;
            }

            State = NodeState.Failure;
            return State;
        }
    }
}