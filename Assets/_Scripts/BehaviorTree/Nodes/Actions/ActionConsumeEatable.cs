using BehaviorTree.Blackboard;
using BehaviorTree.NPCStats;
using BehaviorTree.Core;
using UnityEngine;
using System;
using AddIns;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionConsumeEatable : ActionNode
    {
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly SpitterStats _stats;
        private readonly Rigidbody2D _rigid;
        private readonly Animator _animator;
        private readonly float _timer;
        
        public static event Action OnConsumeEatable;

        public ActionConsumeEatable(SpitterStats stats, Transform transform, Animator animator, IBlackboard blackboard)
        {
            _transform = transform.parent.GetComponent<Transform>();
            _rigid = transform.parent.GetComponent<Rigidbody2D>();
            _blackboard = blackboard;
            _animator = animator;
            _stats = stats;
        }

        public override NodeState Evaluate()
        {
            var player = _blackboard.GetData<Transform>("player");
            var distance = Vector2.Distance(_transform.position, player.position);
            var direction = Vec2.Direction(_transform.position, player.position);
            var step = _stats.SpeedGoToEat * Time.deltaTime;
            Vec2.MoveTo(_transform, player, step);
            Vec2.LookAt(_rigid, direction);
            if (distance <= _stats.StopDistanceEat)
            {
                _rigid.velocity = Vector2.zero;
                Debug.Log("Eating");
                OnConsumeEatable?.Invoke();
                _animator.SetTrigger("IsEatingTrigger");
                
                State = NodeState.Success;
                return State;
            }

            State = NodeState.Failure;
            return State;
        }
    }
}