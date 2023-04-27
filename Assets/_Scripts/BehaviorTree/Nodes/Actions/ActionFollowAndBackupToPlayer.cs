using UnityEngine;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionFollowAndBackupToPlayer : LeafNode
    {
        private const float BackupDistance = 0.5f;
        
        private readonly Transform _transform;
        private readonly Rigidbody2D _rigid;
        private readonly Animator _animator;
        private readonly float _stopDistance;
        private readonly float _speed;

        public ActionFollowAndBackupToPlayer(float speed, float stopDistance, Transform transform)
        {
            _animator = transform.GetComponentInChildren<Animator>();
            _rigid = transform.GetComponent<Rigidbody2D>();
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
                var direction = ((Vector2)player.position - (Vector2)_rigid.transform.position).normalized;
                var step = _speed * Time.deltaTime;
                _transform.position = Vector2.MoveTowards(_transform.position, player.position, step);
                _rigid.transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);
                _animator.SetBool("IsWalking", true);
                
                State = NodeState.Success;
                return State;
            }
            if (distance < _stopDistance - BackupDistance)
            {
                var reverseDirection = ((Vector2)_rigid.transform.position - (Vector2)player.position).normalized;
                var backup = reverseDirection * BackupDistance;
                var step = _speed * Time.deltaTime;
                _transform.position = MathL.MoveAway(_transform.position, backup, step);
                //_transform.position = Vector2.MoveTowards(_transform.position, _transform.position + (Vector3)reverseDirection * _backupDistance, step);
                _rigid.transform.localScale = new Vector3(reverseDirection.x > 0 ? 1 : -1, 1, 1);
                _animator.SetBool("IsWalking", true);
                
                State = NodeState.Success;
                return State;
            }
            
            _animator.SetBool("IsWalking", false);

            State = NodeState.Failure;
            return State;
        }
    }
}