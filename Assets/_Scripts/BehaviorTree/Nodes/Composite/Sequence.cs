using System.Collections.Generic;

namespace BehaviorTree.Nodes.Composite
{
    public class Sequence : Node
    {
        public Sequence() : base() { }
        public Sequence(List<Node> children) : base(children) { }
        
        public override ReturnStat Tick()
        {
            var isRunning = false;
            foreach (var child in Children)
            {
                switch (child.Tick())
                {
                    case ReturnStat.SUCCESS:
                        continue;
                    case ReturnStat.FAILURE:
                        Result = ReturnStat.FAILURE;
                        return Result;
                    case ReturnStat.RUNNING:
                        isRunning = true;
                        continue;
                    default:
                        Result = ReturnStat.SUCCESS;
                        return Result;
                }
            }
            
            Result = isRunning ? ReturnStat.RUNNING : ReturnStat.SUCCESS;
            return Result;
        }
    }
}