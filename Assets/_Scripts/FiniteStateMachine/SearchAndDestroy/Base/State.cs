using FiniteStateMachine.SearchAndDestroy.Controller;

namespace FiniteStateMachine.SearchAndDestroy.Base
{
    public abstract class State
    {
        protected StateController StateController;
        
        public void OnStateEnter(StateController stateController)
        {
            StateController = stateController;
            OnEnter();
        }
        
        protected virtual void OnEnter()
        {
        }
        
        public void OnStateUpdate()
        {
            OnUpdate();
        }
        
        protected virtual void OnUpdate()
        {
        }

        protected bool OnStateCanTransitionTo()
        {
            return OnCanTransitionTo();
        }
        
        protected virtual bool OnCanTransitionTo()
        {
            return false;
        }
        
        public void OnStateExit()
        {
            OnExit();
        }
        
        protected virtual void OnExit()
        {
        }
    }
}