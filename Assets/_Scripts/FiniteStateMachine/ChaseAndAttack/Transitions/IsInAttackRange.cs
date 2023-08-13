using FiniteStateMachine.Base;
using BehaviorTree.Blackboard;
using NpcSettings;
using UnityEngine;

namespace FiniteStateMachine.ChaseAndAttack.Transitions
{
    public class IsInAttackRange : Transition
    {
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly NpcData _stats;
        
        
        public IsInAttackRange(NpcData stats, Transform transform, IBlackboard blackboard) : base(stats, transform)
        {
            _transform = transform.parent;
            _blackboard = blackboard;
            _stats = stats;
        }

        public override bool OnCanTransitionTo()
        {
            var target = GetTarget();
            return target != null && IsDistanceLessThanAttackRange(target);
        }

        private bool IsDistanceLessThanAttackRange(Transform target)
        {
            var distance = Vector2.Distance(_transform.position, target.position);
            if (!(distance < _stats.AttackRadius)) return false;
            
            _stats.IsInAttackPhase = true;
            return true;
        }
        
        private Transform GetTarget()
        {
            return _blackboard.GetData<Transform>(_stats.TargetTag);
        }
    }
}