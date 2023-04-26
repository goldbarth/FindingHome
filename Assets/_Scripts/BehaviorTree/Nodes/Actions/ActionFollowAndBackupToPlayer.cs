using UnityEngine;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionFollowAndBackupToPlayer : LeafNode
    {
        private float _speed;
        private float _stopDistance;
        private float _backupDistance = 0.5f;
        private Transform _transform;
        private Animator _animator;
        private Rigidbody2D _rb;
        
        public ActionFollowAndBackupToPlayer(float speed, float stopDistance, Transform transform)
        {
            _animator = transform.GetComponentInChildren<Animator>();
            _rb = transform.GetComponent<Rigidbody2D>();
            _stopDistance = stopDistance;
            _transform = transform;
            _speed = speed;
        }
        
        public override NodeState Evaluate()
        {
            var player = (Transform)GetData("player");
            var distance = Vector2.Distance(_transform.position, player.position);
            if (distance > _stopDistance)
            {
                var direction = ((Vector2)player.position - (Vector2)_rb.transform.position).normalized;
                var step = _speed * Time.deltaTime;
                _transform.position = Vector2.MoveTowards(_transform.position, player.position, step);
                _rb.transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);
                _animator.SetBool("IsWalking", true);
            }
            else if (distance < _stopDistance - _backupDistance)
            {
                var reverseDirection = ((Vector2)_rb.transform.position - (Vector2)player.position).normalized;
                var backup = reverseDirection * _backupDistance;
                var step = _speed * Time.deltaTime;
                _transform.position = MathL.MoveAway(_transform.position, backup, step);
                //_transform.position = Vector2.MoveTowards(_transform.position, _transform.position + (Vector3)reverseDirection * _backupDistance, step);
                _rb.transform.localScale = new Vector3(reverseDirection.x > 0 ? 1 : -1, 1, 1);
                _animator.SetBool("IsWalking", true);
            }
            else
            {
                _animator.SetBool("IsWalking", false);
            }

            State = NodeState.Success;
            return State;
        }
    }
}