using FiniteStateMachine.ChaseAndAttack.Transitions;
using FiniteStateMachine.FollowPlayer.Transitions;
using FiniteStateMachine.ChaseAndAttack.States;
using FiniteStateMachine.FollowPlayer.States;
using FiniteStateMachine.Shared;
using FiniteStateMachine.Base;
using BehaviorTree.Blackboard;
using BehaviorTree.Behaviors;
using UnityEngine;
using Player;

// Source of inspiration: https://gamedevbeginner.com/state-machines-in-unity-how-and-when-to-use-them/
// https://medium.com/@abdullahahmetaskin/finite-state-machine-and-behavior-tree-fusion-3fcce33566
// Not used the second link, but it's a good read and was letting me do my own implementation.
namespace FiniteStateMachine.Controller
{
    public class StateController : MonoBehaviour
    {
        // Transitions:
        public IsInIdleRange IsInSuspiciousIdleRange;
        public IsInIdleRange IsInFriendlyIdleRange;
        public HasPlayerEdible HasPlayerEdible;
        public IsInAttackRange IsInAttackRange;
        public IsInBackupRange IsInBackupRange;
        public IsTargetInFOV IsPlayerInFOV;
        public IsTargetInFOV IsEnemyInFOV;

        // States:
        public FollowState SuspiciousFollowState;
        public FollowState FriendlyFollowState;
        public EatEdibleState EatEdibleState;
        public IdleState SuspiciousIdleState;
        public IdleState FriendlyIdleState;
        public BackupState BackupState;
        public AttackState AttackState;
        public ChaseState ChaseState;
        
        [HideInInspector] public FriendlyNpcBehavior NpcBehavior;
        private State _currentState;
        
        private void Awake()
        {
            var transform = this.transform;
            var animator = transform.parent.GetComponentInChildren<Animator>();
            NpcBehavior = GetComponent<FriendlyNpcBehavior>();
            var player = FindObjectOfType<PlayerController>();
            var blackboard = new Blackboard();
            
            ChaseState = new ChaseState(NpcBehavior.Stats, transform, blackboard, animator, NpcBehavior.FootstepSound);
            AttackState = new AttackState(NpcBehavior.Stats, transform, blackboard);
            
            EatEdibleState = new EatEdibleState(NpcBehavior.Stats, transform, animator, blackboard);
            BackupState = new BackupState(NpcBehavior.Stats, transform, animator, NpcBehavior.FootstepSound, blackboard);
            SuspiciousFollowState = new FollowState(RangeType.Near, NpcBehavior.Stats, transform, animator, NpcBehavior.FootstepSound, blackboard);
            FriendlyFollowState = new FollowState(RangeType.Protect, NpcBehavior.Stats, transform, animator, NpcBehavior.FootstepSound, blackboard);
            SuspiciousIdleState = new IdleState(RangeType.Near, NpcBehavior.Stats, player, transform, animator, NpcBehavior.FootstepSound, blackboard);
            FriendlyIdleState = new IdleState(RangeType.Protect, NpcBehavior.Stats, player, transform, animator, NpcBehavior.FootstepSound, blackboard);
            
            HasPlayerEdible = new HasPlayerEdible(player);
            IsInBackupRange = new IsInBackupRange(NpcBehavior.Stats, transform, blackboard);
            IsPlayerInFOV = new IsTargetInFOV(TargetType.Player, NpcBehavior.Stats, transform, blackboard);
            IsInFriendlyIdleRange = new IsInIdleRange(RangeType.Protect, NpcBehavior.Stats, transform, blackboard);
            IsInSuspiciousIdleRange = new IsInIdleRange(RangeType.Near, NpcBehavior.Stats, transform, blackboard);
            
            IsEnemyInFOV = new IsTargetInFOV(TargetType.Enemy, NpcBehavior.Stats, transform, blackboard);
            IsInAttackRange = new IsInAttackRange(NpcBehavior.Stats, transform, blackboard);
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