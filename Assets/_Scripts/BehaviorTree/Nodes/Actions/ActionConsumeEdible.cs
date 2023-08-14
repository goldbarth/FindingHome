using BehaviorTree.Blackboard;
using HelpersAndExtensions;
using BehaviorTree.Core;
using NpcSettings;
using UnityEngine;
using System;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionConsumeEdible : ActionNode
    {
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly Rigidbody2D _rigid;
        private readonly Animator _animator;
        private readonly NpcData _stats;
        private readonly float _timer;
        
        private Vector2 _velocity;

        public static event Action OnConsumeEdible;

        public ActionConsumeEdible(NpcData stats, Transform transform, Animator animator, IBlackboard blackboard)
        {
            _transform = transform.parent.GetComponent<Transform>();
            _rigid = transform.parent.GetComponent<Rigidbody2D>();
            _blackboard = blackboard;
            _animator = animator;
            _stats = stats;
        }

        public override NodeState Evaluate()
        {
            var position = _transform.position;
            var player = _blackboard.GetData<Transform>("player");
            var distance = Vector2.Distance(position, player.position);
            var direction = Vec2.Direction(position, player.position);

            _transform.position = Vector2.SmoothDamp(position, player.position, ref _velocity, _stats.SmoothTimeFast);
            Vec2.LookAt(_rigid, direction);
            
            if (distance <= _stats.StopDistanceEat)
            {
                _rigid.velocity = Vector2.zero;
                OnConsumeEdible?.Invoke();
                _animator.SetTrigger("IsEatingTrigger");
                
                State = NodeState.Success;
                return State;
            }

            State = NodeState.Failure;
            return State;
        }
    }
}