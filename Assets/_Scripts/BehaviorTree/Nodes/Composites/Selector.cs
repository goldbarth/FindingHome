﻿using System.Collections.Generic;
using BehaviorTree.Core;

namespace BehaviorTree.Nodes.Composites
{
    public class Selector : CompositeNode
    {
        public Selector() : base() { }
        public Selector(List<BaseNode> children) : base(children) { }

        public override NodeState Evaluate()
        {
            foreach (var child in Children)
            {
                switch (child.Evaluate())
                {
                    case NodeState.Success:
                        State = NodeState.Success;
                        return State;
                    case NodeState.Failure:
                        continue;
                    case NodeState.Running:
                        State = NodeState.Running;
                        return State;
                    default:
                        continue;
                }
            }
            
            State = NodeState.Failure;
            return State;
        }
    }
}