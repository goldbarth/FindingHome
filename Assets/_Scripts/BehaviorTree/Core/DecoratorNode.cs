using System.Collections.Generic;

namespace BehaviorTree.Core
{
    public abstract class DecoratorNode : Node
    {
        protected DecoratorNode() : base() {}
        protected DecoratorNode(List<Node> children)
        {
            foreach(var child in children)
                AddChild(child);
        }
    }
}