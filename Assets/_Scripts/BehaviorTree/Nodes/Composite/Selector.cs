using System.Collections.Generic;

namespace BehaviorTree.Nodes.Composite
{
    public class Selector : Node
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
                        Result = NodeState.SUCCESS;
                        return Result;
                    case NodeState.FAILURE:
                        continue;
                    case NodeState.RUNNING:
                        return Result;
                    default:
                        continue;
                }
            }
            
            Result = NodeState.FAILURE;
            return Result;
        }
    }
}