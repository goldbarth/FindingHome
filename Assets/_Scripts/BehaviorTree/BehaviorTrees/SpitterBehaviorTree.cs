using System.Collections.Generic;
using BehaviorTree.Nodes.Actions;
using BehaviorTree.Nodes.Composite;
using BehaviorTree.Nodes.Conditions;
using BehaviorTree.Nodes.Decorator;
using UnityEngine;

namespace BehaviorTree.BehaviorTrees
{
    public class SpitterBehaviorTree : Tree
    {
        //TODO: testing purpose. entities later.
        [SerializeField] private float _detectionRadiusEnemy = 5f;
        [SerializeField] private float _detectionRadiusPlayer = 7f;
        [SerializeField] private float _attackRadius = 1.5f;
        [SerializeField] private float _stopDistanceTarget = 1.7f;
        [SerializeField] private float _stopDistancePlayer = 3f;
        [SerializeField] private float _speedGoToPlayer = 2.5f;
        [SerializeField] private float _speedPlayerFollow = 6.8f;
        [SerializeField] private float _speedTargetFollow = 5f;
        [SerializeField] private bool _hasEaten = false;
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
                            new CheckHasEaten(),
                            new Inverter(new List<Node>
                            {
                                new Sequence(new List<Node>
                                {
                                    new CheckTargetInFOVRange(_detectionRadiusEnemy, _targetLayer, trans),
                                    new CheckTargetInAttackRange(trans, _attackRadius),
                                    new ActionAttackTarget(trans, _speedTargetFollow)
                                })
                            }),
                            new ActionProtectPlayer(_speedPlayerFollow, _stopDistancePlayer, trans)
                        }
                    ),
                    new Sequence(new List<Node>
                    {
                        new Inverter(new List<Node>
                        {
                            new CheckHasEaten()
                        }),
                        new CheckPlayerInFOVRange(_detectionRadiusPlayer, _playerLayer, trans),
                        new ActionFollowAndBackupPlayer(_speedGoToPlayer, _stopDistancePlayer, trans),
                        new CheckPlayerHasEatable(),
                        new ActionConsumeEatable(5.5f, trans),
                    })
                }
            );
                
                

            return root;
        }
        
        private void OnEnable()
        {
            ActionConsumeEatable.OnConsumeEatableEvent += SetHasEaten;
        }
        
        private void OnDisable()
        {
            ActionConsumeEatable.OnConsumeEatableEvent -= SetHasEaten;
        }

        private void SetHasEaten()
        {
            _hasEaten = true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _detectionRadiusEnemy);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _detectionRadiusPlayer);
        }
    }
}