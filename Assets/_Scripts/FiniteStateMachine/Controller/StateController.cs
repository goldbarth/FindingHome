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
namespace FiniteStateMachine.Controller
{
    public class StateController : MonoBehaviour
    {
        public IsInBackupRange IsInBackupRange;
        public IsInAttackRange IsInAttackRange;
        public IsInIdleRange IsInSuspiciousIdleRange;
        public IsInIdleRange IsInFriendlyIdleRange;
        public IsTargetInFOV IsPlayerInFOV;
        public IsTargetInFOV IsEnemyInFOV;

        public FollowState SuspiciousFollowState;
        public FollowState FriendlyFollowState;
        public IdleState SuspiciousIdleState;
        public IdleState FriendlyIdleState;
        public BackupState BackupState;
        public AttackState AttackState;
        public ChaseState ChaseState;
        
        private State _currentState;
        private Animator _animator;
        
        private void Awake()
        {
            _animator = transform.parent.GetComponentInChildren<Animator>();
            var npcBehavior = GetComponent<FriendlyNpcBehavior>();
            var player = FindObjectOfType<PlayerController>();
            var blackboard = new Blackboard();
            
            ChaseState = new ChaseState(npcBehavior.Stats, transform, blackboard, _animator, npcBehavior.FootstepSound);
            AttackState = new AttackState(npcBehavior.Stats, transform, blackboard);
            
            BackupState = new BackupState(npcBehavior.Stats, transform, _animator, npcBehavior.FootstepSound, blackboard);
            SuspiciousFollowState = new FollowState(RangeType.Near, npcBehavior.Stats, transform, _animator, npcBehavior.FootstepSound, blackboard);
            FriendlyFollowState = new FollowState(RangeType.Protect, npcBehavior.Stats, transform, _animator, npcBehavior.FootstepSound, blackboard);
            SuspiciousIdleState = new IdleState(RangeType.Near, npcBehavior.Stats, player, transform, _animator, npcBehavior.FootstepSound, blackboard);
            FriendlyIdleState = new IdleState(RangeType.Protect, npcBehavior.Stats, player, transform, _animator, npcBehavior.FootstepSound, blackboard);
            
            IsPlayerInFOV = new IsTargetInFOV(TargetType.Player, npcBehavior.Stats, transform, blackboard);
            IsEnemyInFOV = new IsTargetInFOV(TargetType.Enemy, npcBehavior.Stats, transform, blackboard);
            IsInBackupRange = new IsInBackupRange(npcBehavior.Stats, transform, blackboard);
            IsInAttackRange = new IsInAttackRange(npcBehavior.Stats, transform, blackboard);
            IsInFriendlyIdleRange = new IsInIdleRange(RangeType.Protect, npcBehavior.Stats, transform, blackboard);
            IsInSuspiciousIdleRange = new IsInIdleRange(RangeType.Near, npcBehavior.Stats, transform, blackboard);
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