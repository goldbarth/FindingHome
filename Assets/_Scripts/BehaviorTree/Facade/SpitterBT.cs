using BehaviorTree.Nodes.Composites;
using BehaviorTree.Nodes.Conditions;
using BehaviorTree.Nodes.Decorator;
using System.Collections.Generic;
using BehaviorTree.Nodes.Actions;
using BehaviorTree.Entities;
using Player.PlayerData;
using BehaviorTree.Core;
using Player;
using UnityEngine;

namespace BehaviorTree.Facade
{
    public class SpitterBT : FacadeBase
    {
        private readonly EatablesCount _eatables;
        private readonly SpitterEntity _entity;
        private readonly Transform _transform;
        private readonly bool _isChangingColor;
        
        public SpitterBT(SpitterEntity entity, Transform transform, EatablesCount eatables, bool isChangingColor)
        {
            _isChangingColor = isChangingColor;
            _transform = transform;
            _eatables = eatables;
            _entity = entity;
        }

        protected internal override Node GetRoot()
        {
             var blackboard = new Blackboard.Blackboard();
             var player = Object.FindObjectOfType<PlayerController>();

             var root = new Selector(new List<Node>
                {
                    new Sequence(new List<Node>
                        {
                            new CheckIfFriendlyNPCHasEaten(_eatables),
                            new Inverter(new List<Node>
                            {
                                new Sequence(new List<Node>
                                {
                                    new CheckForObjectInFOVRange(_entity._targetTag, _entity._detectionRadiusEnemy, _entity._targetLayer, _transform, blackboard),
                                    new ActionGoToTarget(_entity._speedTargetFollow, .4f, _transform, blackboard),
                                    new CheckIfTargetInAttackRange(_transform, _entity._attackRadius, blackboard),
                                    new ActionAttackTarget(_transform, _entity._speedTargetFollow, blackboard)
                                })
                            }),
                            new Inverter(new List<Node>
                            {
                                new CheckIfInAttackPhase()
                            }),
                            new CheckForObjectInFOVRange(_entity._playerTag, _entity._detectionRadiusPlayer, _entity._playerLayer, _transform, blackboard),
                            //new Inverter(new List<Node>
                            //    {
                            //        new CheckIfPlayerWasCommanding(),
                            //        new CheckIfFarRange(),
                            //        new ActionFollowAndBackupToPlayer(_entity._speedGoToPlayer, _entity._detectionRadiusPlayer, _entity._farRangeStopDistance, _transform, blackboard)
                            //    }
                            //    ),
                            new ActionProtectPlayer(_entity._speedPlayerFollow, _entity._stopDistancePlayerProtect, _transform, blackboard)
                        }
                    ),
                    new Sequence(new List<Node>
                    {
                        new Inverter(new List<Node>
                        {
                            new CheckIfFriendlyNPCHasEaten(_eatables)
                        }),
                        new CheckForObjectInFOVRange(_entity._playerTag, _entity._detectionRadiusPlayer, _entity._playerLayer, _transform, blackboard),
                        new ActionFollowAndBackupToPlayer(_entity._speedGoToPlayer, _entity._detectionRadiusPlayer, _entity._nearRangeStopDistance, _transform, blackboard),
                        new CheckIfPlayerHasEatable(_eatables),
                        new ActionConsumeEatable(5.5f, _transform, blackboard),
                        new ActionChangeSpriteColor(_transform, _entity._animatorController, _isChangingColor)
                    })
                }
            );
            
            return root;
        }

       
    }
}