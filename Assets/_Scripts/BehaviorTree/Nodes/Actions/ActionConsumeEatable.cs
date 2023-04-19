using UnityEngine;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionConsumeEatable : LeafNode
    {
        public delegate void ConsumeEatable();
        public static event ConsumeEatable OnConsumeEatableEvent;
        
        private float _speed;
        private float _timer;
        private float _duration = 2f;
        private bool _isEating = false;
        private Rigidbody2D _rb;
        private Animator _animator;
        private Transform _transform;

        public ActionConsumeEatable(float speed, Component component)
        {
            _animator = component.GetComponentInChildren<Animator>();
            _transform = component.GetComponent<Transform>();
            _rb = component.GetComponent<Rigidbody2D>();
            _speed = speed;
        }

        public override NodeState Evaluate()
        {
            var target = (Transform)GetData("target");
            var targetDir = ((Vector2)target.position - (Vector2)_transform.position).normalized;
            var distance = Vector2.Distance(_transform.position, target.position);
            var step = _speed * Time.deltaTime;
            _transform.position = Vector2.MoveTowards(_transform.position, target.position, step);
            _rb.transform.localScale = new Vector3(targetDir.x > 0 ? 1 : -1, 1, 1);

            if (distance <= 1f)
            {
                _rb.velocity = Vector2.zero;

                Debug.Log("Eating");
                OnConsumeEatableEvent?.Invoke();
                _animator.SetBool("IsEating", true);
                
                State = NodeState.Success;
                return State;
                
                //if (!_isEating)
                //{
                //    Debug.Log("Eating");
                //    OnConsumeEatableEvent?.Invoke();
                //    _animator.SetBool("IsEating", true);
                //    _isEating = true;
                //    _timer = 0;
                //    
                //    State = NodeState.Success;
                //    return State;
                //}
////
                //_timer += Time.deltaTime;
////
                //if (_timer >= _duration)
                //{
                //    Debug.Log("Finished eating");
                //    _animator.SetBool("IsEating", false);
                //    _isEating = false;
                //    
                //    State = NodeState.Success;
                //    return State;
                //}
//
                //State = NodeState.Running;
                //return State;
            }

            State = NodeState.Failure;
            return State;
        }
    }
}