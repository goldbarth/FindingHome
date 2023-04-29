﻿using AddIns;
using BehaviorTree.Blackboard;
using BehaviorTree.Core;
using Enemies;
using UnityEngine;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionAttackTarget : ActionNode
    {
        private const float AttackTime = .8f;
        
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly Rigidbody2D _rigid;
        private readonly Animator _animator;
        private readonly float _speed;
        
        private Transform _lastTarget;
        private Transform _target;
        private Summoner _summoner;
        private float _attackCounter;

        public static bool IsInAttackPhase { get; private set; }

        public ActionAttackTarget() : base() { }
        public ActionAttackTarget(Transform transform, float speed, IBlackboard blackboard) : base()
        {
            _animator = transform.GetComponentInChildren<Animator>();
            _rigid = transform.GetComponent<Rigidbody2D>();
            _blackboard = blackboard;
            _transform = transform;
            _speed = speed;
        }

        public override NodeState Evaluate()
        {
            Debug.Log("Checking if in attack phase: " + IsInAttackPhase);
            try
            {
                if(!_blackboard.ContainsKey("target"))
                {
                    State = NodeState.Failure;
                    return State;
                }
                
                var target = _blackboard.GetData<Transform>("target");
                
                // This is to prevent the enemy from switching targets mid-attack
                _lastTarget = target == _target ? _lastTarget : _target;
                _target = target;

                // Set the target component if it isn´t already set
                _summoner ??= _target.GetComponent<Summoner>();
                
                var direction = Vec2.Direction(_transform.position, target.position);
                var step = _speed * Time.deltaTime;
                Vec2.MoveTo(_transform, target, step);
                Vec2.LookAt(_rigid, direction);

                if (_attackCounter >= AttackTime)
                {
                    Debug.Log("Attacking target");
                    var enemyIsDead = _summoner.TakeHit();
                    if (enemyIsDead)
                    {
                        _blackboard.ClearData("target");
                        _animator.SetBool("IsAttacking", false);

                        State = NodeState.Failure;
                        return State;
                    }

                    _attackCounter = 0f;
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