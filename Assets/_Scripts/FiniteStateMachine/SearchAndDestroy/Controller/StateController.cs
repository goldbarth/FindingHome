using BehaviorTree.Blackboard;
using BehaviorTree.NPCStats;
using FiniteStateMachine.SearchAndDestroy.Base;
using FiniteStateMachine.SearchAndDestroy.States;
using UnityEngine;

namespace FiniteStateMachine.SearchAndDestroy.Controller
{
    public class StateController : MonoBehaviour

    {
        [SerializeField] private SpitterStats _stats;
        [SerializeField] private AudioSource _audioSource;
        
        private readonly Blackboard _blackboard = new();
        private Animator _animator;
        
        public AttackState AttackState;
        public ChaseState ChaseState;
        
        private State _currentState;
        
        private void Awake()
        {
            _animator = transform.parent.GetComponentInChildren<Animator>();
            AttackState = new AttackState(_stats, transform, _blackboard);
            ChaseState = new ChaseState(_stats, transform, _blackboard, _animator, _audioSource);
        }
        
        private void Start()
        {
            ChangeState(ChaseState);
        }
        
        private void Update()
        {
            _currentState?.OnStateUpdate();
        }

        public void ChangeState(State newState)
        {
            _currentState?.OnStateExit();

            _currentState = newState;
            _currentState.OnStateEnter(this);
        }
    }
}