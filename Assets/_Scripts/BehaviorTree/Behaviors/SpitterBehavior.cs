using System;
using System.Collections.Generic;
using Tree = BehaviorTree.Core.Tree;
using BehaviorTree.Entities;
using BehaviorTree.Facade;
using BehaviorTree.Core;
using BehaviorTree.Nodes.Actions;
using BehaviorTree.Nodes.Composites;
using BehaviorTree.Nodes.Conditions;
using BehaviorTree.Nodes.Decorator;
using Player;
using Player.PlayerData;
using UnityEngine;

namespace BehaviorTree.Behaviors
{
    public class SpitterBehavior : Tree
    {
        [SerializeField] private SpitterEntity _entity;
        [SerializeField] private bool _isChangingColor;
        [SerializeField] private Animator _animator;       
        
        private readonly IEatables _eatables;


        private Dictionary<SpitterAnimationEvents, Action> _animationEventDictionary;

        protected override void Start()
        {
            base.Start();

            _animationEventDictionary = new Dictionary<SpitterAnimationEvents, Action>();
            _animationEventDictionary.Add(SpitterAnimationEvents.ChangeController, ChangeToFriendlyAnimator);
        }

        protected override Node CreateTree()
        {
  
            //var tree = new SpitterBT(_entity, transform, SpitterEntity.Eatables, _isChangingColor);
            //return tree.GetRoot();
            
             var blackboard = new Blackboard.Blackboard();
             var player = FindObjectOfType<PlayerController>();

             var root = new Selector(new List<Node>
                {
                    new Sequence(new List<Node>
                        {
                            new CheckIfFriendlyNPCHasEaten(_eatables),
                            new Inverter(new List<Node>
                            {
                                new Sequence(new List<Node>
                                {
                                    new CheckForObjectInFOVRange(_entity._targetTag, _entity._detectionRadiusEnemy, _entity._targetLayer, transform, blackboard),
                                    new ActionGoToTarget(_entity._speedTargetFollow, .4f, transform, blackboard),
                                    new CheckIfTargetInAttackRange(transform, _entity._attackRadius, blackboard),
                                    new ActionAttackTarget(transform, _entity._speedTargetFollow, blackboard)
                                })
                            }),
                            new Inverter(new List<Node>
                            {
                                new CheckIfInAttackPhase()
                            }),
                            new CheckForObjectInFOVRange(_entity._playerTag, _entity._detectionRadiusPlayer, _entity._playerLayer, transform, blackboard),
                            //new Inverter(new List<Node>
                            //    {
                            //        new CheckIfPlayerWasCommanding(),
                            //        new CheckIfFarRange(),
                            //        new ActionFollowAndBackupToPlayer(_entity._speedGoToPlayer, _entity._detectionRadiusPlayer, _entity._farRangeStopDistance, _transform, blackboard)
                            //    }
                            //    ),
                            new ActionProtectPlayer(_entity._speedPlayerFollow, _entity._stopDistancePlayerProtect, transform, blackboard)
                        }
                    ),
                    new Sequence(new List<Node>
                    {
                        new Inverter(new List<Node>
                        {
                            new CheckIfFriendlyNPCHasEaten(_eatables)
                        }),
                        new CheckForObjectInFOVRange(_entity._playerTag, _entity._detectionRadiusPlayer, _entity._playerLayer, transform, blackboard),
                        new ActionFollowAndBackupToPlayer(_entity._speedGoToPlayer, _entity._detectionRadiusPlayer, _entity._nearRangeStopDistance, transform, blackboard),
                        new CheckIfPlayerHasEatable(_eatables),
                        new ActionConsumeEatable(5.5f, transform, blackboard),
                        new ActionChangeSpriteColor(transform, _entity._animatorController, _isChangingColor)
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
            if(!_isChangingColor) return;
            _animator.runtimeAnimatorController = _entity._animatorController;
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _entity._detectionRadiusEnemy);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _entity._detectionRadiusPlayer);
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, _entity._attackRadius);
        }
#endif
    }
}