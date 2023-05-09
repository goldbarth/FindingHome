using BehaviorTree.Nodes.Composites;
using BehaviorTree.Nodes.Conditions;
using BehaviorTree.Nodes.Decorator;
using BehaviorTree.Nodes.Actions;
using System.Collections.Generic;
using BehaviorTree.NPCStats;
using BehaviorTree.Core;
using UnityEngine;
using System;
using Player;

namespace BehaviorTree.Behaviors
{
    public class SpitterBehavior : BaseTree
    {
        [SerializeField] private SpitterStats _stats;
        [SerializeField] private bool _isChangingColor;
        
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
            _stats.IsInAttackPhase = false;
            _animationEventDictionary = new Dictionary<SpitterAnimationEvents, Action>();
            _animationEventDictionary.Add(SpitterAnimationEvents.ChangeController, ChangeToFriendlyAnimator);
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
                                    new CheckForObjectInFOVRange(FOVType.Target, _stats, transform, blackboard),
                                    new ActionGoToTarget(_stats, transform, _animator,blackboard),
                                    new CheckIfTargetInAttackRange(_stats, transform, blackboard),
                                    new ActionAttackTarget(_stats, transform, _animator, blackboard)
                                })
                            }),
                            new Inverter(new List<BaseNode>
                            {
                                new CheckIfInAttackPhase(_stats)
                            }),
                            new CheckForObjectInFOVRange(FOVType.Player, _stats, transform, blackboard),
                            new ActionFollowPlayer(RangeType.Protect, _stats, transform, _animator, blackboard),
                            new ActionIdle(RangeType.Protect, _stats, transform, _animator, blackboard),
                            //new Inverter(new List<Node>
                            //    {
                            //        new CheckIfPlayerWasCommanding(),
                            //        new CheckIfFarRange(),
                            //        new ActionFollowAndBackupToPlayer(_entity._speedGoToPlayer, _entity._detectionRadiusPlayer, _entity._farRangeStopDistance, _transform, blackboard)
                            //    }
                            //    ),
                        }
                    ),
                    new Sequence(new List<BaseNode>
                    {
                        new Inverter(new List<BaseNode>
                        {
                            new CheckIfFriendlyNPCHasEaten(_stats)
                        }),
                        new CheckForObjectInFOVRange(FOVType.Player, _stats, transform, blackboard),
                        new Selector(new List<BaseNode>
                        {
                            new ActionFollowPlayer(RangeType.Near, _stats, transform, _animator, blackboard),
                            new ActionIdle(RangeType.Near, _stats, transform, _animator, blackboard),
                            new ActionBackupPlayer(_stats, transform, _animator, blackboard),
                        }),
                        new CheckIfPlayerHasEatable(player),
                        new ActionConsumeEatable(_stats, transform, _animator, blackboard),
                    })
                }
            );

            return root;
        }

        public void CallAnimationEvent(SpitterAnimationEvents eventKey)
        {
            _animationEventDictionary[eventKey].Invoke();
        }

        public void ChangeToFriendlyAnimator()
        {
            if (!_isChangingColor) return;
            _animator.runtimeAnimatorController = _stats.AnimatorController;
        }

        private void SetFriendly()
        {
            _stats.HasEaten = true;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _stats.DetectionRadiusEnemy);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _stats.DetectionRadiusPlayer);
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, _stats.AttackRadius);
        }
#endif
    }
}