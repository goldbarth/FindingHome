using BehaviorTree.Blackboard;
using BehaviorTree.Core;
using UnityEngine;
using AddIns;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionConsumeEatable : ActionNode
    {
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly Rigidbody2D _rigid;
        private readonly Animator _animator;
        private readonly float _speed;
        private readonly float _timer;
        private readonly float _stopDistance;
        
        public delegate void ConsumeEatable();
        public static event ConsumeEatable OnConsumeEatableEvent;

        public ActionConsumeEatable(float speed, float stopDistance, Transform transform, Animator animator, IBlackboard blackboard)
        {
            _animator = transform.parent.GetComponentInChildren<Animator>();
            _transform = transform.parent.GetComponent<Transform>();
            _rigid = transform.parent.GetComponent<Rigidbody2D>();
            _stopDistance = stopDistance;
            _blackboard = blackboard;
            _speed = speed;
        }

        public override NodeState Evaluate()
        {
            if(!_blackboard.ContainsKey("player"))
            {
                State = NodeState.Failure;
                return State;
            }
                
            var player = _blackboard.GetData<Transform>("player");
            var distance = Vector2.Distance(_transform.position, player.position);
            var direction = Vec2.Direction(_transform.position, player.position);
            var step = _speed * Time.deltaTime;
            Vec2.MoveTo(_transform, player, step);
            Vec2.LookAt(_rigid, direction);
            if (distance <= _stopDistance)
            {
                _rigid.velocity = Vector2.zero;
                Debug.Log("Eating");
                OnConsumeEatableEvent?.Invoke();
                _animator.SetTrigger("IsEatingTrigger");
                
                State = NodeState.Success;
                return State;
            }

            State = NodeState.Failure;
            return State;
        }
    }
}