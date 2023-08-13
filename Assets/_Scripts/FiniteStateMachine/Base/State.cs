using FiniteStateMachine.Controller;

namespace FiniteStateMachine.Base
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
        
        public void OnStateExit()
        {
            OnExit();
        }
        
        protected virtual void OnExit()
        {
        }
    }
}