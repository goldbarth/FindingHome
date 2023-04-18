using System.Collections.Generic;

namespace BehaviorTree
{
    public class CompositeNode : Node
    {
        public CompositeNode()
        {
            Parent = null;
        }
        
        public CompositeNode(List<Node> children)
        {
            foreach(var child in children)
                AddChild(child);
        }
    }
}