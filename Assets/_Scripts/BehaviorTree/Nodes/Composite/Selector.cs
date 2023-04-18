using System.Collections.Generic;

namespace BehaviorTree.Nodes.Composite
{
    public class Selector : CompositeNode
    {
        public Selector() : base() { }
        public Selector(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            foreach (var child in Children)
            {
                switch (child.Evaluate())
                {
                    case NodeState.SUCCESS:
                        State = NodeState.SUCCESS;
                        return State;
                    case NodeState.FAILURE:
                        continue;
                    case NodeState.RUNNING:
                        State = NodeState.RUNNING;
                        return State;
                    default:
                        continue;
                }
            }
            
            State = NodeState.FAILURE;
            return State;
        }
    }
}