using Tree = BehaviorTree.Core.Tree;
using BehaviorTree.Nodes.Composites;
using BehaviorTree.Nodes.Conditions;
using BehaviorTree.Nodes.Decorator;
using BehaviorTree.Nodes.Actions;
using System.Collections.Generic;
using BehaviorTree.NPCStats;
using BehaviorTree.Core;
using UnityEngine;
using Player;
using System;

namespace BehaviorTree.Behaviors
{
    public class SpitterBehavior : Tree
    {
        [SerializeField] private SpitterStats _stats;
        [SerializeField] private bool _isChangingColor;
        
        private Dictionary<SpitterAnimationEvents, Action> _animationEventDictionary;
        private Animator _animator;

        private void OnEnable()
        {
            ActionConsumeEatable.OnConsumeEatableEvent += SetFriendly;
        }

        private void OnDisable()
        {
            ActionConsumeEatable.OnConsumeEatableEvent -= SetFriendly;
        }

        private void Awake()
        {
            _animator = transform.parent.GetComponentInChildren<Animator>();
        }

        protected override void Start()
        {
            base.Start();

            _stats._hasEaten = false;
            _stats._isInAttackPhase = false;
            _animationEventDictionary = new Dictionary<SpitterAnimationEvents, Action>();
            _animationEventDictionary.Add(SpitterAnimationEvents.ChangeController, ChangeToFriendlyAnimator);
        }

        protected override BaseNode CreateTree()
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
                                    new CheckForObjectInFOVRange(_stats._targetTag, _stats._detectionRadiusEnemy, _stats._targetLayer, transform, blackboard),
                                    new ActionGoToTarget(_stats, transform, _animator,blackboard),
                                    new CheckIfTargetInAttackRange(_stats, transform, blackboard),
                                    new ActionAttackTarget(_stats, transform, _animator, blackboard)
                                })
                            }),
                            new Inverter(new List<BaseNode>
                            {
                                new CheckIfInAttackPhase(_stats)
                            }),
                            new CheckForObjectInFOVRange(_stats._playerTag, _stats._detectionRadiusPlayer, _stats._playerLayer, transform, blackboard),
                            //new Inverter(new List<Node>
                            //    {
                            //        new CheckIfPlayerWasCommanding(),
                            //        new CheckIfFarRange(),
                            //        new ActionFollowAndBackupToPlayer(_entity._speedGoToPlayer, _entity._detectionRadiusPlayer, _entity._farRangeStopDistance, _transform, blackboard)
                            //    }
                            //    ),
                            new ActionProtectPlayer(_stats, transform, _animator, blackboard)
                        }
                    ),
                    new Sequence(new List<BaseNode>
                    {
                        new Inverter(new List<BaseNode>
                        {
                            new CheckIfFriendlyNPCHasEaten(_stats)
                        }),
                        new CheckForObjectInFOVRange(_stats._playerTag, _stats._detectionRadiusPlayer, _stats._playerLayer, transform, blackboard),
                        new ActionFollowAndBackupToPlayer(_stats, transform, _animator, blackboard),
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
            _animator.runtimeAnimatorController = _stats._animatorController;
        }

        private void SetFriendly()
        {
            _stats._hasEaten = true;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _stats._detectionRadiusEnemy);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _stats._detectionRadiusPlayer);
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, _stats._attackRadius);
        }
#endif
    }
}