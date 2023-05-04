using System.Collections.Generic;

namespace BehaviorTree.Core
{
    public abstract class BaseNode
    {
        protected readonly List<BaseNode> Children = new();

        protected NodeState State;

        protected BaseNode() : base() {}
        
        //protected BaseNode(List<BaseNode> children)
        //{
        //    Children.AddRange(children);
        //}

        public virtual NodeState Evaluate() => NodeState.Failure;
    }
}