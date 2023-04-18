using System.Collections.Generic;
using BehaviorTree.Nodes.Action;
using BehaviorTree.Nodes.Actions;
using BehaviorTree.Nodes.Composite;
using BehaviorTree.Nodes.Conditions;
using UnityEngine;
using UnityEngine.Serialization;
using DataPersistence;

namespace BehaviorTree.Behaviors
{
    public class SpitterBehavior : Tree
    {
        //TODO: testing purpose. entities later.
        [SerializeField] private float detectionRadiusEnemy = 5f;
        [SerializeField] private float detectionRadiusPlayer = 7f;
        [SerializeField] private float attackRadius = 1.5f;
        [SerializeField] private float stopDistanceTarget = 1.7f;
        [SerializeField] private float stopDistancePlayer = 1f;
        [SerializeField] private float speedGoToPlayer = 2.5f;
        [SerializeField] private float speedPlayerFollow = 6.8f;
        //[SerializeField] private LayerMask targetLayer;
        [SerializeField] private LayerMask playerLayer;
        
        private GameData _gameData;

        protected override Node CreateTree()
        {
            var gameData = new GameData();
            //const float jumpInterval = 1.2f;
            var trans = transform;
            var root = new Selector(new List<Node>
            {
                new Sequence(new List<Node>
                {
                    new CheckTargetInFOVRange(detectionRadiusPlayer, playerLayer, trans),
                    new ActionFollowTarget(speedGoToPlayer, stopDistancePlayer, trans),
                    new Sequence(new List<Node>
                    {
                        new CheckPlayerHasEatable(),
                        new ActionFollowTarget(speedPlayerFollow, stopDistancePlayer, trans),
                        
                    })
                }),
                //new Sequence(new List<Node>
                //{
                //    new CheckTargetInFOVRange(detectionRadiusEnemy, targetLayer, trans),
                //    new ActionFollowTarget(stopDistance, trans),
                //    new ActionTryReachTarget(jumpForce, jumpInterval, trans),
                //    new ActionIdle(trans)
                //}),
            });
            
            return root;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadiusEnemy);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, detectionRadiusPlayer);
        }
    }
}