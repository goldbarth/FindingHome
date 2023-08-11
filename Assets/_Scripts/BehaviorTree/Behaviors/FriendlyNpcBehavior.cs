using FiniteStateMachine.SearchAndDestroy.Controller;
using System.Collections.Generic;
using BehaviorTree.Nodes.Composites;
using BehaviorTree.Nodes.Conditions;
using BehaviorTree.Nodes.Decorator;
using BehaviorTree.Nodes.Actions;
using BehaviorTree.Nodes.Custom;
using BehaviorTree.NPCStats;
using BehaviorTree.Core;
using UnityEngine;
using System;
using Player;

namespace BehaviorTree.Behaviors
{
    public class FriendlyNpcBehavior : BaseTree
    {
        [Header("Stats and References")]
        [SerializeField] private SpitterStats _stats;
        [SerializeField] private StateController _stateController;
        [Header("Audio")]
        [SerializeField] private AudioSource _attackSound;
        [SerializeField] private AudioSource _footstepSound;

        private Dictionary<SpitterAnimationEvents, Action> _animationEventDictionary;
        private Animator _animator;

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
        }

        protected override void Start()
        {
            base.Start();

            _stats.HasEaten = false;
            _stats.IsFarRange = false;
            _stats.IsInAttackPhase = false;
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
                    new CheckIfFriendlyNPCHasEaten(_stats),
                    new Inverter(new List<BaseNode>
                    {
                        new Sequence(new List<BaseNode>
                        {
                            new StateMachineNode(_stateController),
                            // ATTACK STATE MACHINE HERE!!!
                        })
                    }),
                    new Inverter(new List<BaseNode>
                    {
                        new CheckIfInAttackPhase(_stats)
                    }),
                    new CheckForTargetInFOVRange(TargetType.Player, _stats, transform, blackboard),
                    new Selector(new List<BaseNode>
                    {
                        new ActionFollowPlayer(RangeType.Protect, _stats, transform, _animator, _footstepSound, blackboard),
                        new ActionIdle(RangeType.Protect, _stats, player, transform, _animator, _footstepSound, blackboard),
                    })
                }),
                new Sequence(new List<BaseNode>
                {
                    new Inverter(new List<BaseNode>
                    {
                        new CheckIfFriendlyNPCHasEaten(_stats)
                    }),
                    new CheckForTargetInFOVRange(TargetType.Player, _stats, transform, blackboard),
                    new Selector(new List<BaseNode>
                    {
                        new ActionFollowPlayer(RangeType.Near, _stats, transform, _animator, _footstepSound, blackboard),
                        new ActionIdle(RangeType.Near, _stats, player, transform, _animator, _footstepSound, blackboard),
                        new ActionBackupPlayer(_stats, transform, _animator, _footstepSound, blackboard),
                    }),
                    new CheckIfPlayerHasEatable(player),
                    new ActionConsumeEatable(_stats, transform, _animator, blackboard),
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
            _animator.runtimeAnimatorController = _stats.AnimatorController;
        }
        
        private void PlayAttackSound()
        {
            Debug.Log("Play Attack Sound");
            _attackSound.PlayOneShot(_attackSound.clip);
        }

        private void SetFriendly()
        {
            _stats.HasEaten = true;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = _stats.ShowPlayerDetectionRadius ? Color.green : Color.clear;
            Gizmos.DrawWireSphere(transform.position, _stats.DetectionRadiusPlayer);
            Gizmos.color = _stats.ShowIdleSpace ? Color.magenta : Color.clear;
            Gizmos.DrawWireSphere(transform.position, _stats.DetectionRadiusPlayer - _stats.NearRangeStopDistance - _stats.MaxBackupDistance);
            Gizmos.DrawWireSphere(transform.position, _stats.DetectionRadiusPlayer - _stats.NearRangeStopDistance - _stats.DistanceBetweenOffset);
            Gizmos.color = _stats.ShowEnemyDetectionRadius ? Color.red : Color.clear;
            Gizmos.DrawWireSphere(transform.position, _stats.DetectionRadiusEnemy);
            Gizmos.color = _stats.ShowBackupRadius ? Color.yellow : Color.clear;
            Gizmos.DrawWireSphere(transform.position, _stats.DetectionRadiusPlayer - _stats.NearRangeStopDistance - _stats.MaxBackupDistance);
            Gizmos.color = _stats.ShowFollowRadius ? Color.blue : Color.clear;
            Gizmos.DrawWireSphere(transform.position, _stats.DetectionRadiusPlayer - _stats.NearRangeStopDistance - _stats.DistanceBetweenOffset);
        }
#endif
    }
}

// new CheckForTargetInFOVRange(FOVType.Target, _stats, transform, blackboard),
// new ActionChaseTarget(_stats, transform, _animator, blackboard, _footstepSound),
// new CheckIfTargetInAttackRange(_stats, transform, blackboard),
// new ActionAttackTarget(_stats, transform, _animator, blackboard)