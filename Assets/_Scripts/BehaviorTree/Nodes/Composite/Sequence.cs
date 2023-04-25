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
                    case NodeState.Success:
                        continue;
                    case NodeState.Failure:
                        State = NodeState.Failure;
                        return State;
                    case NodeState.Running:
                        anyChildIsRunning = true;
                        continue;
                    default:
                        State = NodeState.Success;
                        return State;
                }
            }
            
            State = anyChildIsRunning ? NodeState.Running : NodeState.Success;
            return State;
        }
    }

    public class Copy1OfSequence : CompositeNode
    {
        public Copy1OfSequence() : base() { }
        public Copy1OfSequence(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            var anyChildIsRunning = false;
            foreach (var child in Children)
            {
                switch (child.Evaluate())
                {
                    case NodeState.Success:
                        continue;
                    case NodeState.Failure:
                        State = NodeState.Failure;
                        return State;
                    case NodeState.Running:
                        anyChildIsRunning = true;
                        continue;
                    default:
                        State = NodeState.Success;
                        return State;
                }
            }

            State = anyChildIsRunning ? NodeState.Running : NodeState.Success;
            return State;
        }
    }

    public class CopyOfSequence : CompositeNode
    {
        public CopyOfSequence() : base() { }
        public CopyOfSequence(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            var anyChildIsRunning = false;
            foreach (var child in Children)
            {
                switch (child.Evaluate())
                {
                    case NodeState.Success:
                        continue;
                    case NodeState.Failure:
                        State = NodeState.Failure;
                        return State;
                    case NodeState.Running:
                        anyChildIsRunning = true;
                        continue;
                    default:
                        State = NodeState.Success;
                        return State;
                }
            }

            State = anyChildIsRunning ? NodeState.Running : NodeState.Success;
            return State;
        }
    }
}