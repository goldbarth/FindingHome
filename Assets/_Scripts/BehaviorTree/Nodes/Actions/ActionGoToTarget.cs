using UnityEngine;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionGoToTarget : LeafNode
    {
        private readonly Transform _transform;
        private readonly Animator _animator;
        private readonly Rigidbody2D _rb;
        private readonly float _stopDistance;
        private readonly float _speed;
        
        public ActionGoToTarget(float speed, float stopDistance, Transform transform)
        {
            _animator = transform.GetComponentInChildren<Animator>();
            _rb = transform.GetComponent<Rigidbody2D>();
            _stopDistance = stopDistance;
            _transform = transform;
            _speed = speed;
        }
        
        public override NodeState Evaluate()
        {
            var target = (Transform)GetData("target");
            var targetDir = ((Vector2)target.position - (Vector2)_rb.transform.position).normalized;
            var distance = Vector2.Distance(_transform.position, target.position);
            if (distance > _stopDistance)
            {
                _animator.SetBool("IsWalking", true);
                var step = _speed * Time.deltaTime;
                _transform.position = Vector2.MoveTowards(_transform.position, target.position, step);
                _rb.transform.localScale = new Vector3(targetDir.x > 0 ? 1 : -1, 1, 1);
                
                State = NodeState.Success;
                return State; 
            }
            
            State = NodeState.Failure;
            return State;
        }
    }
}