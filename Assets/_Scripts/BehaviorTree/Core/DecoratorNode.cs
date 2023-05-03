using System.Collections.Generic;

namespace BehaviorTree.Core
{
    public abstract class DecoratorNode : Node
    {
        protected DecoratorNode() : base() {}
        protected DecoratorNode(List<Node> children)
        {
            Children.AddRange(children);
        }
    }
}