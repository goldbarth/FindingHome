using System.Collections.Generic;

namespace BehaviorTree.Core
{
    public abstract class CompositeNode : BaseNode
    {
         protected CompositeNode() : base() {}
        
        protected CompositeNode(List<BaseNode> children)
        {
            Children.AddRange(children);
        }
    }
}