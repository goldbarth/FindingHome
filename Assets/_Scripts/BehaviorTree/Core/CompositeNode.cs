using System.Collections.Generic;

namespace BehaviorTree.Core
{
    public abstract class CompositeNode : Node
    {
         protected CompositeNode() : base() {}
        
        protected CompositeNode(List<Node> children)
        {
            foreach(var child in children)
                AddChild(child);
        }
    }
}