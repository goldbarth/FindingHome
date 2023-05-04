using BehaviorTree.Blackboard;
using BehaviorTree.Core;
using UnityEngine;
using Enemies;
using AddIns;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionAttackTarget : ActionNode
    {
        
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly Rigidbody2D _rigid;
        private readonly Animator _animator;
        private readonly float _speed;
        private readonly float _timerReset;
        private readonly int _attackDamage;
        
        private Transform _lastTarget;
        private Transform _target;
        private Summoner _summoner;
        private float _attackCounter;
        private float _attackTimer;
        
        public ActionAttackTarget() : base() { }
        public ActionAttackTarget(Transform transform, float speed, float attackTimer, int attackDamage, Animator animator, IBlackboard blackboard)
        {
            _animator = transform.parent.GetComponentInChildren<Animator>(); 
            _rigid = transform.parent.GetComponent<Rigidbody2D>();
            _transform = transform.parent;
            _attackDamage = attackDamage;
            _attackTimer = attackTimer;
            _timerReset = attackTimer;
            _blackboard = blackboard;
            _speed = speed;
        }

        public override NodeState Evaluate()
        {
            try
            {
                if(!_blackboard.ContainsKey("target"))
                {
                    State = NodeState.Failure;
                    return State;
                }
                
                var target = _blackboard.GetData<Transform>("target");

                _lastTarget = target == _target ? _lastTarget : _target;
                _target = target;
                
                _summoner = _target.GetComponent<Summoner>();

                var direction = Vec2.Direction(_transform.position, target.position);
                var step = _speed * Time.deltaTime;
                Vec2.MoveTo(_transform, target, step);
                Vec2.LookAt(_rigid, direction);
                
                if (_attackTimer > _attackCounter)
                {
                    _attackTimer -= Time.deltaTime;
                    State = NodeState.Running;
                    return State;
                }

                _animator.SetBool("IsAttacking", true);
                var enemyIsDead = _summoner.TakeDamage(_attackDamage);
                if (enemyIsDead)
                {
                    GameManager.Instance.IsInAttackPhase = false;
                    Debug.Log("Is target id in BB: " + _blackboard.TryGetId("target", out var id) + "If so, the ID is:" + id);
                    _blackboard.ClearData("target");
                    Debug.Log("Is the target ID cleared after death: " + !_blackboard.ContainsId(id));
                    _animator.SetBool("IsAttacking", false);

                    State = NodeState.Failure;
                    return State;
                }

                _attackTimer = _timerReset;

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