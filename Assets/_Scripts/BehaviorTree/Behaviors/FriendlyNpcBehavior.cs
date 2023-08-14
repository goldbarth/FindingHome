using System.Collections.Generic;
using FiniteStateMachine.FollowPlayer.States;
using FiniteStateMachine.Controller;
using BehaviorTree.Nodes.Composites;
using BehaviorTree.Nodes.Conditions;
using BehaviorTree.Nodes.FsmFusion;
using BehaviorTree.Nodes.Decorator;
using BehaviorTree.Core;
using NpcSettings;
using UnityEngine;
using System;

namespace BehaviorTree.Behaviors
{
    public class FriendlyNpcBehavior : BaseTree
    {
        [Header("Settings")]
        [field:SerializeField] public NpcData Stats;
        [Header("Audio")]
        [field:SerializeField] public AudioSource FootstepSound;
        [SerializeField] private AudioSource _attackSound;

        private Dictionary<SpitterAnimationEvents, Action> _animationEventDictionary;
        private Animator _animator;
        
        private MulticastDelegate _animationEvent;

        private void OnEnable()
        {
            EatEdibleState.OnConsumeEdible += SetFriendly;
        }

        private void OnDisable()
        {
            EatEdibleState.OnConsumeEdible -= SetFriendly;
        }

        private void Awake()
        {
            _animator = transform.parent.GetComponentInChildren<Animator>();
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
            var stateController = GetComponent<StateController>();
            var root = new Selector(new List<BaseNode>
            {
                new Sequence(new List<BaseNode>
                {
                    new CheckIfFriendlyNpcHasEaten(Stats),
                    new ChaseAndAttackFsm(stateController),
                    new Inverter(new List<BaseNode>
                    {
                        new CheckIfInAttackPhase(Stats)
                    }),
                    new FollowPlayerFsm(stateController, RangeType.Protect),
                }),
                new Sequence(new List<BaseNode>
                {
                    new Inverter(new List<BaseNode>
                    {
                        new CheckIfFriendlyNpcHasEaten(Stats)
                    }),
                    new FollowPlayerFsm(stateController, RangeType.Near),
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