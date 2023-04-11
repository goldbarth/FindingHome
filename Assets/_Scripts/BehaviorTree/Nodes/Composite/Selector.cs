using System.Collections.Generic;

namespace BehaviorTree.Nodes.Composite
{
    public class Selector : Node
    {
        public Selector() : base() { }
        public Selector(List<Node> children) : base(children) { }

        public override ReturnStat Tick()
        {
            foreach (var child in Children)
            {
                switch (child.Tick())
                {
                    case ReturnStat.SUCCESS:
                        Result = ReturnStat.SUCCESS;
                        return Result;
                    case ReturnStat.FAILURE:
                        continue;
                    case ReturnStat.RUNNING:
                        return Result;
                    default:
                        continue;
                }
            }
            
            Result = ReturnStat.FAILURE;
            return Result;
        }
    }
}