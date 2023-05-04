using UnityEditor.Animations;
using BehaviorTree.Core;
using UnityEngine;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionChangeSpriteColor : ActionNode
    {
        private readonly AnimatorController _newController;
        private readonly Animator _animator;
        private readonly bool _isChangingColor;

        private bool _hasChanged;

        public ActionChangeSpriteColor(Component component, AnimatorController newController, Animator animator, bool isChangingColor = true)
        {
            _isChangingColor = isChangingColor;
            _newController = newController;
            _animator = animator;
        }

        public override NodeState Evaluate()
        {
            if(!_isChangingColor) return State = NodeState.Success;
            if (!_hasChanged)
            {
                //_animator.SetTrigger("IsEatingTrigger");
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