using Tree = BehaviorTree.Core.Tree;
using BehaviorTree.Nodes.Conditions;
using BehaviorTree.Nodes.Composites;
using BehaviorTree.Nodes.Decorator;
using BehaviorTree.Nodes.Actions;
using System.Collections.Generic;
using BehaviorTree.Core;
using Player.PlayerData;
using UnityEngine;
using Player;

namespace BehaviorTree.BehaviorTrees
{
    public class SpitterBehaviorTree : Tree
    {
        [SerializeField] private Entity.Entity _entity;

        protected override Node CreateTree()
        {
            var eatables = FindObjectOfType<PlayerController>().GetComponent<EatablesCount>();
            var blackboard = new Blackboard.Blackboard();
            var trans = transform;
            
            var root = new Selector(new List<Node>
                {
                    new Sequence(new List<Node>
                        {
                            new CheckIfFriendlyNPCHasEaten(eatables),
                            new Inverter(new List<Node>
                            {
                                new Sequence(new List<Node>
                                {
                                    new CheckForObjectInFOVRange(_entity._targetTag, _entity._detectionRadiusEnemy, _entity._targetLayer, trans, blackboard),
                                    new ActionGoToTarget(_entity._speedTargetFollow, .4f, trans, blackboard),
                                    new CheckIfTargetInAttackRange(trans, _entity._attackRadius, blackboard),
                                    new ActionAttackTarget(trans, _entity._speedTargetFollow, blackboard)
                                })
                            }),
                            new Inverter(new List<Node>
                            {
                                new CheckIfInAttackPhase()
                            }),
                            new CheckForObjectInFOVRange(_entity._playerTag, _entity._detectionRadiusPlayer, _entity._playerLayer, trans, blackboard),
                            new ActionProtectPlayer(_entity._speedPlayerFollow, _entity._stopDistancePlayerProtect, trans, blackboard)
                        }
                    ),
                    new Sequence(new List<Node>
                    {
                        new Inverter(new List<Node>
                        {
                            new CheckIfFriendlyNPCHasEaten(eatables)
                        }),
                        new CheckForObjectInFOVRange(_entity._playerTag, _entity._detectionRadiusPlayer, _entity._playerLayer, trans, blackboard),
                        new ActionFollowAndBackupToPlayer(_entity._speedGoToPlayer, _entity._stopDistancePlayer, trans, blackboard),
                        new CheckIfPlayerHasEatable(eatables),
                        new ActionConsumeEatable(5.5f, trans, blackboard),
                        //new ActionChangeFriendlyNPCSpriteColor(trans, _friendlyAnimController)
                    })
                }
            );
            
            return root;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _entity._detectionRadiusEnemy);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _entity._detectionRadiusPlayer);
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, _entity._attackRadius);
        }
    }
}