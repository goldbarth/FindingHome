using NpcSettings;
using UnityEngine;

namespace FiniteStateMachine.Base
{
    public abstract class Transition
    {
        protected NpcData Stats;
        protected Transform Transform;
        
        protected Transition(NpcData stats, Transform transform)
        {
            Stats = stats;
            Transform = transform;
        }
        
        public bool OnStateCanTransitionTo()
        {
            return OnCanTransitionTo();
        }

        public virtual bool OnCanTransitionTo()
        {
            return false;
        }
    }
}