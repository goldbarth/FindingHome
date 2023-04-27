using UnityEngine;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionProtectPlayer : LeafNode
    {
        private readonly Transform _transform;
        private readonly Animator _animator;
        private readonly Rigidbody2D _rigid;
        private readonly float _stopDistance;
        private readonly float _speed;
        
        public ActionProtectPlayer(float speed, float stopDistance, Transform transform)
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