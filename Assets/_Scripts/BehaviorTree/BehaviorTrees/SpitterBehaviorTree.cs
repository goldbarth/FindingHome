using System;
using System.Collections.Generic;
using BehaviorTree.Nodes.Actions;
using BehaviorTree.Nodes.Composite;
using BehaviorTree.Nodes.Conditions;
using BehaviorTree.Nodes.Decorator;
using Enemies;
using UnityEngine;

namespace BehaviorTree.BehaviorTrees
{
    public class SpitterBehaviorTree : Tree
    {
        //TODO: testing purpose. entities later.
        [SerializeField] private string _playerTag = "player";
        [SerializeField] private string _targetTag = "target";
        [SerializeField] private float _detectionRadiusEnemy = 5f;
        [SerializeField] private float _detectionRadiusPlayer = 7f;
        [SerializeField] private float _attackRadius = 2f;
        [SerializeField] private float _stopDistancePlayer = 3f;
        [SerializeField] private float _stopDistancePlayerProtect = 1.8f;
        [SerializeField] private float _speedGoToPlayer = 2.5f;
        [SerializeField] private float _speedPlayerFollow = 6.8f;
        [SerializeField] private float _speedTargetFollow = 5f;
        [SerializeField] private LayerMask _targetLayer;
        [SerializeField] private LayerMask _playerLayer;

        protected override Node CreateTree()
        {
            //const float jumpInterval = 1.2f;
            var trans = transform;
            var root = new Selector(new List<Node>
                {
                    new Sequence(new List<Node>
                        {
                            new CheckIfFriendlyNPCHasEaten(),
                            new Inverter(new List<Node>
                            {
                                new Sequence(new List<Node>
                                {
                                    new CheckForObjectInFOVRange(_targetTag, _detectionRadiusEnemy, _targetLayer, trans),
                                    new CheckIfTargetInAttackRange(trans, _attackRadius),
                                    new ActionAttackTarget(trans, _speedTargetFollow)
                                })
                            }),
                            new Inverter(new List<Node>
                            {
                                new CheckIfInAttackPhase()
                            }),
                            new CheckForObjectInFOVRange(_playerTag, _detectionRadiusPlayer, _playerLayer, trans),
                            new ActionProtectPlayer(_speedPlayerFollow, _stopDistancePlayerProtect, trans)
                        }
                    ),
                    new Sequence(new List<Node>
                    {
                        new Inverter(new List<Node>
                        {
                            new CheckIfFriendlyNPCHasEaten()
                        }),
                        new CheckForObjectInFOVRange(_playerTag, _detectionRadiusPlayer, _playerLayer, trans),
                        new ActionFollowAndBackupToPlayer(_speedGoToPlayer, _stopDistancePlayer, trans),
                        new CheckIfPlayerHasEatable(),
                        new ActionConsumeEatable(5.5f, trans),
                    })
                }
            );
            
            return root;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _detectionRadiusEnemy);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _detectionRadiusPlayer);
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, _attackRadius);
        }
    }
}