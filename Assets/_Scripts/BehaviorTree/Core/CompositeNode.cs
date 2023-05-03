using System.Collections.Generic;

namespace BehaviorTree.Core
{
    public abstract class CompositeNode : Node
    {
         protected CompositeNode() : base() {}
        
        protected CompositeNode(List<Node> children)
        {
            Children.AddRange(children);
        }
    }
}