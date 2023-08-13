using System.Collections.Generic;
using BehaviorTree.Nodes.Composites;
using BehaviorTree.Nodes.Conditions;
using BehaviorTree.Nodes.Decorator;
using BehaviorTree.Nodes.Actions;
using BehaviorTree.Core;
using NpcSettings;
using UnityEngine;
using System;
using BehaviorTree.Nodes.StateMachine;
using FiniteStateMachine.Controller;
using Player;

namespace BehaviorTree.Behaviors
{
    public class FriendlyNpcBehavior : BaseTree
    {
        [Header("Settings")]
        [field:SerializeField] public NpcData Stats;
        [Header("Audio")]
        [SerializeField] private AudioSource _attackSound;
        [field:SerializeField] public AudioSource FootstepSound;

        private Dictionary<SpitterAnimationEvents, Action> _animationEventDictionary;
        private StateController _stateController;
        private Animator _animator;
        
        private MulticastDelegate _animationEvent;

        private void OnEnable()
        {
            ActionConsumeEatable.OnConsumeEatable += SetFriendly;
        }

        private void OnDisable()
        {
            ActionConsumeEatable.OnConsumeEatable -= SetFriendly;
        }

        private void Awake()
        {
            _animator = transform.parent.GetComponentInChildren<Animator>();
            _stateController = GetComponent<StateController>();
        }

        protected override void Start()
        {
            base.Start();

            Stats.HasEaten = false;
            Stats.IsFarRange = false;
            Stats.IsInAttackPhase = false;
            _animationEventDictionary = new Dictionary<SpitterAnimationEvents, Action>
            {
                { SpitterAnimationEvents.ChangeController, ChangeToFriendlyAnimator },
                { SpitterAnimationEvents.PlayAttackSound, PlayAttackSound }
            };
        }

        protected override BaseNode SetupTree()
        {
            var blackboard = new Blackboard.Blackboard();
            var player = FindObjectOfType<PlayerController>();
            

            var root = new Selector(new List<BaseNode>
            {
                new Sequence(new List<BaseNode>
                {
                    new CheckIfFriendlyNPCHasEaten(Stats),
                    new ChaseAndAttackFsm(_stateController),
                    new Inverter(new List<BaseNode>
                    {
                        new CheckIfInAttackPhase(Stats)
                    }),
                    new FollowPlayerFsm(_stateController, RangeType.Protect),
                }),
                new Sequence(new List<BaseNode>
                {
                    new Inverter(new List<BaseNode>
                    {
                        new CheckIfFriendlyNPCHasEaten(Stats)
                    }),
                    // new CheckForTargetInFOVRange(TargetType.Player, Stats, transform, blackboard),
                    // new Selector(new List<BaseNode>
                    // {
                    //     new ActionFollowPlayer(RangeType.Near, Stats, transform, _animator, FootstepSound, blackboard),
                    //     new ActionIdle(RangeType.Near, Stats, player, transform, _animator, FootstepSound, blackboard),
                    //     new ActionBackupPlayer(Stats, transform, _animator, FootstepSound, blackboard),
                    // }),
                    new FollowPlayerFsm(_stateController, RangeType.Near),
                    new CheckIfPlayerHasEatable(player),
                    new ActionConsumeEatable(Stats, transform, _animator, blackboard),
                })
            });

            return root;
        }

        public void CallAnimationEvent(SpitterAnimationEvents eventKey)
        {
            _animationEventDictionary[eventKey].Invoke();
        }

        private void ChangeToFriendlyAnimator()
        {
            _animator.runtimeAnimatorController = Stats.AnimatorController;
        }
        
        private void PlayAttackSound()
        {
            Debug.Log("Play Attack Sound");
            _attackSound.PlayOneShot(_attackSound.clip);
        }

        private void SetFriendly()
        {
            Stats.HasEaten = true;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Stats.ShowPlayerDetectionRadius ? Color.green : Color.clear;
            Gizmos.DrawWireSphere(transform.position, Stats.DetectionRadiusPlayer);
            Gizmos.color = Stats.ShowIdleSpace ? Color.magenta : Color.clear;
            Gizmos.DrawWireSphere(transform.position, Stats.DetectionRadiusPlayer - Stats.NearRangeStopDistance - Stats.MaxBackupDistance);
            Gizmos.DrawWireSphere(transform.position, Stats.DetectionRadiusPlayer - Stats.NearRangeStopDistance - Stats.DistanceBetweenOffset);
            Gizmos.color = Stats.ShowEnemyDetectionRadius ? Color.red : Color.clear;
            Gizmos.DrawWireSphere(transform.position, Stats.DetectionRadiusEnemy);
            Gizmos.color = Stats.ShowBackupRadius ? Color.yellow : Color.clear;
            Gizmos.DrawWireSphere(transform.position, Stats.DetectionRadiusPlayer - Stats.NearRangeStopDistance - Stats.MaxBackupDistance);
            Gizmos.color = Stats.ShowFollowRadius ? Color.blue : Color.clear;
            Gizmos.DrawWireSphere(transform.position, Stats.DetectionRadiusPlayer - Stats.NearRangeStopDistance - Stats.DistanceBetweenOffset);
        }
#endif
    }
}

// new CheckForTargetInFOVRange(FOVType.Target, _stats, transform, blackboard),
// new ActionChaseTarget(_stats, transform, _animator, blackboard, _footstepSound),
// new CheckIfTargetInAttackRange(_stats, transform, blackboard),
// new ActionAttackTarget(_stats, transform, _animator, blackboard)