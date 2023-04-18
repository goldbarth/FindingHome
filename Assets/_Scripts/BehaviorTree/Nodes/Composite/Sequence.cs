using System.Collections.Generic;

namespace BehaviorTree.Nodes.Composite
{
    public class Sequence : CompositeNode
    {
        public Sequence() : base() { }
        public Sequence(List<Node> children) : base(children) { }
        
        public override NodeState Evaluate()
        {
            var anyChildIsRunning = false;
            foreach (var child in Children)
            {
                switch (child.Evaluate())
                {
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.FAILURE:
                        State = NodeState.FAILURE;
                        return State;
                    case NodeState.RUNNING:
                        anyChildIsRunning = true;
                        continue;
                    default:
                        State = NodeState.SUCCESS;
                        return State;
                }
            }
            
            State = anyChildIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            return State;
        }
    }
}