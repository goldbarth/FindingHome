using UnityEngine;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionFollowTarget : LeafNode
    {
        private float _speed = 3f;
        private float _stopDistance;
        private Transform _transform;
        private Animator _animator;
        
        public ActionFollowTarget(float speed, float stopDistance, Transform transform)
        {
            _animator = transform.GetComponentInChildren<Animator>();
            _transform = transform;
            _stopDistance = stopDistance;
            _speed = speed;
        }
        
        public override NodeState Evaluate()
        {
            var target = (Transform)GetData("target");
            var distance = Vector2.Distance(_transform.position, target.position);
            // var distanceY = target.position.y - target.position.y;
            if (distance > _stopDistance)
            {
                _animator.SetBool("IsWalking", true);
                var step = _speed * Time.deltaTime;
                
                _transform.position = Vector2.MoveTowards(_transform.position, target.position, step);

                State = NodeState.RUNNING;
                return State;
            }

            State = NodeState.FAILURE;
            return State;
        }
    }
}