
using UnityEngine;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionAttackTarget : LeafNode
    {
        private Transform _transform;
        private Animator _animator;
        private Rigidbody2D _rb;
        private float _speed;
        private float _attackTime = 1f;
        private float _attackCounter;
        
        public ActionAttackTarget(Transform transform, float speed)
        {
            _speed = speed;
            _transform = transform;
            _rb = transform.GetComponent<Rigidbody2D>();
            _animator = transform.GetComponentInChildren<Animator>();
        }
        
        public override NodeState Evaluate()
        {
            var target = (Transform)GetData("target");
            var direction = ((Vector2)target.position - (Vector2)_transform.position).normalized;
            var distance = Vector2.Distance(_transform.position, target.position);
            var step = _speed * Time.deltaTime;
            if (distance > 1f)
            {
                Debug.Log("Attacking target");
                _transform.position = Vector2.MoveTowards(_transform.position, target.position, step);
                _rb.transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);
                _animator.SetBool("IsAttacking", true);
                
                State = NodeState.Success;
                return State;
                _attackCounter += Time.deltaTime;
                if (_attackCounter >= _attackTime)
                {
                    _animator.SetBool("IsAttacking", true);
                    _attackCounter = 0f;
                }
            }

            State = NodeState.Failure;
            return State;
        }
    }
}