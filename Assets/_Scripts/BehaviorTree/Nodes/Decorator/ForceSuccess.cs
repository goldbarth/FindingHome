using System.Collections.Generic;
using BehaviorTree.Core;

namespace BehaviorTree.Nodes.Decorator
{
    public class ForceSuccess : DecoratorNode
    {
        public ForceSuccess() : base() { }
        public ForceSuccess(List<BaseNode> children) : base(children) { }

        public override NodeState Evaluate()
        {
            foreach (var child in Children)
            {
                switch (child.Evaluate())
                {
                    case NodeState.Success:
                        State = NodeState.Success;
                        return State;
                    case NodeState.Failure:
                        State = NodeState.Success;
                        return State;
                    case NodeState.Running:
                        State = NodeState.Running;
                        return State;
                    default:
                        State = NodeState.Success;
                        return State;
                }
            }

            State = NodeState.Success;
            return State;
        }
    }
}