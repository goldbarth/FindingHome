namespace FiniteStateMachine.Base
{
    public abstract class Transition
    {
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