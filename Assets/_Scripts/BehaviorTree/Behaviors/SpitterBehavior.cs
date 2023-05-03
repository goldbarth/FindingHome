using Tree = BehaviorTree.Core.Tree;
using BehaviorTree.Entities;
using BehaviorTree.Facade;
using BehaviorTree.Core;
using UnityEngine;

namespace BehaviorTree.Behaviors
{
    public class SpitterBehavior : Tree
    {
        [SerializeField] private SpitterEntity _entity;
        [SerializeField] private bool _isChangingColor;

        protected override Node CreateTree()
        {
            var tree = new SpitterBT(_entity, transform, SpitterEntity.Eatables, _isChangingColor);
            return tree.GetRoot();
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