using UnityEngine;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionTryReachTarget : LeafNode
    {
        private readonly Animator _animator;
        private readonly Rigidbody2D _rb;
        private readonly float _jumpForce;
        private readonly float _interval;

        private float _timer;
        
        public ActionTryReachTarget(float jumpForce, float interval, Component component)
        {
            _animator = component.GetComponentInChildren<Animator>();
            _rb = component.GetComponent<Rigidbody2D>();
            _jumpForce = jumpForce;
            _interval = interval;
        }

        public override NodeState Evaluate()
        {
            var target = (Transform)GetData("target");
            var targetDir = ((Vector2)target.position - (Vector2)_rb.transform.position).normalized;

            _timer += Time.deltaTime;
            if (_timer >= _interval)
            {
                _animator.SetBool("IsAttacking", true);
                _rb.velocity = new Vector2(_rb.velocity.x, 0);
                _rb.velocity += targetDir * _jumpForce;
                _timer = 0;

                State = NodeState.Running;
                return State;
            }

            State = NodeState.Failure;
            return State;
        }
    }
}