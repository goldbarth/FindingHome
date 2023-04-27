using UnityEngine;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionConsumeEatable : LeafNode
    {
        private readonly Transform _transform;
        private readonly Animator _animator;
        private readonly Rigidbody2D _rb;
        private readonly float _speed;
        private readonly float _timer;
        
        public delegate void ConsumeEatable();
        public static event ConsumeEatable OnConsumeEatableEvent;

        public ActionConsumeEatable(float speed, Component component)
        {
            _animator = component.GetComponentInChildren<Animator>();
            _transform = component.GetComponent<Transform>();
            _rb = component.GetComponent<Rigidbody2D>();
            _speed = speed;
        }

        public override NodeState Evaluate()
        {
            var player = (Transform)GetData("player");
            if (player is null)
            {
                State = NodeState.Failure;
                return State;
            }
            
            var distance = Vector2.Distance(_transform.position, player.position);
            var direction = ((Vector2)player.position - (Vector2)_rb.transform.position).normalized;
            var step = _speed * Time.deltaTime;
            _transform.position = Vector2.MoveTowards(_transform.position, player.position, step);
            _rb.transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);

            if (distance <= 1.2f)
            {
                _rb.velocity = Vector2.zero;

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