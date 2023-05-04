using System.Collections.Generic;
using BehaviorTree.Core;

namespace BehaviorTree.Nodes.Decorator
{
    public class Inverter : DecoratorNode
    {
        public Inverter() : base() { }
        public Inverter(List<BaseNode> children) : base(children) { }

        public override NodeState Evaluate()
        {
            foreach (var child in Children)
            {
                switch (child.Evaluate())
                {
                    case NodeState.Success:
                        State = NodeState.Failure;
                        return State;
                    case NodeState.Failure:
                        State = NodeState.Success;
                        return State;
                    case NodeState.Running:
                        State = NodeState.Running;
                        return State;
                    default:
                        State = NodeState.Failure;
                        return State;
                }
            }
            
            State = NodeState.Failure;
            return State;
        }
    }
}