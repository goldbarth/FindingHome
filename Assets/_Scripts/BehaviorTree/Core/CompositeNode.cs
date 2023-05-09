using System.Collections.Generic;

namespace BehaviorTree.Core
{
    public abstract class CompositeNode : BaseNode
    {
         protected CompositeNode() : base() {}
        
         protected CompositeNode(List<BaseNode> children) : base(children)
         {
             Children.AddRange(children);
         }
    }
}