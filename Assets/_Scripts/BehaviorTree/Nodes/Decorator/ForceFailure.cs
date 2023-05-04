using System.Collections.Generic;
using BehaviorTree.Core;

namespace BehaviorTree.Nodes.Decorator
{
    public class ForceFailure : DecoratorNode
    {
        public ForceFailure() : base() { }
        public ForceFailure(List<BaseNode> children) : base(children) { }

        public override NodeState Evaluate()
        {
            var state = Children[0].Evaluate();
            if (state == NodeState.Running)
            {
                State = NodeState.Running;
                return State;
            }
            
            State = NodeState.Failure;
            return State;
        }
    }
}