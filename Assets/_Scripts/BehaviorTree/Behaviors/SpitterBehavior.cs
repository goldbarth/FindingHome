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
        [SerializeField] private float _detectionRadiusEnemy = 5f;
        [SerializeField] private float _detectionRadiusPlayer = 7f;
        [SerializeField] private float _attackRadius = 1.5f;
        [SerializeField] private float _stopDistanceTarget = 1.7f;
        [SerializeField] private float _stopDistancePlayer = .8f;
        [SerializeField] private float _speedGoToPlayer = 2.5f;
        [SerializeField] private float _speedPlayerFollow = 6.8f;
        [SerializeField] bool _hasEaten = false;
        //[SerializeField] private LayerMask targetLayer;
        [SerializeField] private LayerMask _playerLayer;
        
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
                    new CheckTargetHasEaten(_hasEaten),
                    new ActionFollowTarget(_speedPlayerFollow, _stopDistancePlayer, trans),
                }),
                new Sequence(new List<Node>
                {
                    new CheckTargetInFOVRange(_detectionRadiusPlayer, _playerLayer, trans, CheckType.PlayerInFOVRange, _hasEaten),
                    new ActionFollowTarget(_speedGoToPlayer, _stopDistancePlayer, trans),
                    new Sequence(new List<Node>
                    {
                        new CheckPlayerHasEatable(),
                        new ActionConsumeEatable(5.5f, transform),
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