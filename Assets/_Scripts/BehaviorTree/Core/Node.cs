using System.Collections.Generic;

namespace BehaviorTree.Core
{
    public abstract class Node
    {
        protected readonly List<Node> Children = new();

        protected NodeState State;

        protected Node() : base() {}
        
        protected void AddChild(Node node)
        {
            Children.Add(node);
        }

        protected Node(List<Node> children)
        {
            foreach(var child in children)
                AddChild(child);
        }

        public virtual NodeState Evaluate() => NodeState.Failure;
    }
}