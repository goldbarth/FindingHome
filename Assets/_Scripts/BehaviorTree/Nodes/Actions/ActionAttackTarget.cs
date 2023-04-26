using Enemies;
using UnityEngine;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionAttackTarget : LeafNode
    {
        private readonly Transform _transform;
        private readonly Animator _animator;
        private readonly Rigidbody2D _rb;
        private readonly float _attackTime = 1f;
        private readonly float _speed;
        
        private Transform _lastTarget;
        private Transform _target;
        private Summoner _summoner;
        private float _attackCounter;

        public bool IsAttacking { get; private set; }

        public ActionAttackTarget() : base() { }
        public ActionAttackTarget(Transform transform, float speed)
        {
            _speed = speed;
            _transform = transform;
            _rb = transform.GetComponent<Rigidbody2D>();
            _animator = transform.GetComponentInChildren<Animator>();
        }

        public override NodeState Evaluate()
        {
            try
            {
                var target = (Transform)GetData("target");
                
                // This is to prevent the enemy from switching targets mid-attack
                _lastTarget = target == _target ? _lastTarget : _target;
                _target = target;

                // Set the target component if it isn´t already set
                _summoner ??= _target.GetComponent<Summoner>();
                
                var direction =((Vector2)target.position - (Vector2)_rb.transform.position).normalized;
                var step = _speed * Time.deltaTime;
                
                _transform.position = Vector2.MoveTowards(_transform.position, target.position, step);
                _rb.transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);

                if (_attackCounter >= _attackTime)
                {
                    IsAttacking = true;
                    Debug.Log("Attacking target");
                    var enemyIsDead = _summoner.TakeHit();
                    if (enemyIsDead)
                    {
                        ClearData("target");
                        _animator.SetBool("IsAttacking", false);
                    }
                    else
                    {
                        _attackCounter = 0f;
                    }
                }
                else
                {
                    _attackCounter += Time.deltaTime;
                }

                State = NodeState.Running;
                return State;
            }
            catch
            {

                State = NodeState.Failure;
                return State;
            }
        }
    }
}