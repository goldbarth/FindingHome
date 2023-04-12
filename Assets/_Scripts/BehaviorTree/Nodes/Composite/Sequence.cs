using System.Collections.Generic;

namespace BehaviorTree.Nodes.Composite
{
    public class Sequence : Node
    {
        public Sequence() : base() { }
        public Sequence(List<Node> children) : base(children) { }
        
        public override NodeState Evaluate()
        {
            var isRunning = false;
            foreach (var child in Children)
            {
                switch (child.Evaluate())
                {
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.FAILURE:
                        Result = NodeState.FAILURE;
                        return Result;
                    case NodeState.RUNNING:
                        isRunning = true;
                        continue;
                    default:
                        Result = NodeState.SUCCESS;
                        return Result;
                }
            }
            
            Result = isRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            return Result;
        }
    }
}