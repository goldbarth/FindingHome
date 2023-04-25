using System.Collections.Generic;

namespace BehaviorTree
{
    public class DecoratorNode : Node
    {
        public DecoratorNode()
        {
            Parent = null;
        }
        
        public DecoratorNode(List<Node> children)
        {
            foreach(var child in children)
                AddChild(child);
        }
    }
}