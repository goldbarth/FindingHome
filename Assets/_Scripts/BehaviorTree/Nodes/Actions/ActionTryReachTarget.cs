using UnityEngine;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionTryReachTarget : LeafNode
    {
        private float _timer;
        private float _jumpForce;
        private float _interval;
        private Rigidbody2D _rb;
        private Animator _animator;

        public ActionTryReachTarget(float jumpForce, float interval, Component component)
        {
            _jumpForce = jumpForce;
            _interval = interval;
            _rb = component.GetComponent<Rigidbody2D>();
            _animator = component.GetComponentInChildren<Animator>();
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

                State = NodeState.RUNNING;
                return State;
            }

            State = NodeState.FAILURE;
            return State;
        }
    }
}