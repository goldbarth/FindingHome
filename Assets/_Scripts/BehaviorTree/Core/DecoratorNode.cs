using System.Collections.Generic;

namespace BehaviorTree.Core
{
    public abstract class DecoratorNode : BaseNode
    {
        protected DecoratorNode() : base() {}
        protected DecoratorNode(List<BaseNode> children) : base(children)
        {
            Children.AddRange(children);
        }
    }
}