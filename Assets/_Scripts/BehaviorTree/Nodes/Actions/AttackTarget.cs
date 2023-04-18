
namespace BehaviorTree.Nodes.Action
{
    public class AttackTarget : Node
    {
        public override NodeState Evaluate()
        {
            return NodeState.FAILURE;
        }
    }
}