using BehaviorTree.Blackboard;
using FiniteStateMachine.Base;
using HelpersAndExtensions;
using NpcSettings;
using UnityEngine;

namespace FiniteStateMachine.FollowPlayer.Transitions
{
    public class IsInBackupRange : Transition
    {
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly NpcData _stats;

        private Vector2 _velocity;

        public IsInBackupRange(NpcData stats, Transform transform, IBlackboard blackboard)
        {
            _transform = transform.parent;
            _blackboard = blackboard;
            _stats = stats;
        }

        public override bool OnCanTransitionTo()
        {
            var player = GetTarget();
            if(player == null) return false;
            return Vec2.BackupDistance(_stats, _transform, player);
        }

        private Transform GetTarget()
        {
            return _blackboard.GetData<Transform>(_stats.PlayerTag);
        }
    }
}