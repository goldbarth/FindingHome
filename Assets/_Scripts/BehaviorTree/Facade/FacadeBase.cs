using BehaviorTree.Core;

namespace BehaviorTree.Facade
{
    public abstract class FacadeBase
    {
        protected internal abstract Node GetRoot();
    }
}