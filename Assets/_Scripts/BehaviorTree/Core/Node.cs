using System.Collections.Generic;

namespace BehaviorTree.Core
{
    public abstract class Node
    {
        protected readonly List<Node> Children = new();

        protected NodeState State;

        protected Node() : base() {}
        
        protected Node(List<Node> children)
        {
            Children.AddRange(children);
        }

        public virtual NodeState Evaluate() => NodeState.Failure;

        protected void AddChild(Node node)
        {
            Children.Add(node);
        }
    }
}