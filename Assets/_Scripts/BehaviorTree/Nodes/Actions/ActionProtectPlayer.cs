using UnityEngine;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionProtectPlayer : LeafNode
    {
        private float _speed;
        private float _stopDistance;
        private float _backupDistance = 0.5f;
        private Transform _transform;
        private Animator _animator;
        private Rigidbody2D _rb;
        
        public ActionProtectPlayer(float speed, float stopDistance, Transform transform)
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
            if (player is null)
            {
                State = NodeState.Failure;
                return State;
            }
            var distance = Vector2.Distance(_transform.position, player.position);
            if (distance > _stopDistance)
            {
                // The player is far away, move towards them
                var direction = ((Vector2)player.position - (Vector2)_transform.position).normalized;
                var step = _speed * Time.deltaTime;
                _transform.position = Vector2.MoveTowards(_transform.position, player.position, step);
                _rb.transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);
                _animator.SetBool("IsWalking", true);
            }
            else
            {
                // The object is at the desired distance from the player
                _animator.SetBool("IsWalking", false);
            }

            State = NodeState.Success;
            return State;
        }
    }
}