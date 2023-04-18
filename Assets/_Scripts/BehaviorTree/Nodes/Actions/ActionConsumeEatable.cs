﻿using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionConsumeEatable : LeafNode
    {
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

            if (distance <= .8f)
            {
                _rb.velocity = Vector2.zero;

                if (!_isEating)
                {
                    _animator.SetBool("IsEating", true);
                    _isEating = true;
                    _timer = 0;
                }

                _timer += Time.deltaTime;

                if (_timer >= _duration)
                {
                    _animator.SetBool("IsEating", false);
                    _isEating = false;
                    
                    State = NodeState.RUNNING;
                    return State;
                }

                State = NodeState.RUNNING;
                return State;
            }

            _isEating = false;
            //_animator.SetBool("IsEating", false);

            State = NodeState.RUNNING;
            return State;
        }
    }
}