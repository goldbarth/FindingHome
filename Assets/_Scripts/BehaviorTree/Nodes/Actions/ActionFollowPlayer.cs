﻿using System;
using BehaviorTree.Blackboard;
using BehaviorTree.NPCStats;
using BehaviorTree.Core;
using UnityEngine;
using AddIns;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionFollowPlayer : ActionNode
    {
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly SpitterStats _stats;
        private readonly ActionBackupPlayer _backup;
        private readonly Rigidbody2D _rigid;
        private readonly Animator _animator;
        private readonly float _range;

        private float _currentSpeed;

        public ActionFollowPlayer(Enum rangeType,SpitterStats stats, Transform transform, Animator animator, IBlackboard blackboard)
        {
            switch (rangeType)
            {
                case RangeType.Near:
                    _range = stats.NearRangeStopDistance;
                    break;
                case RangeType.Far:
                    _range = stats.FarRangeStopDistance;
                    break;
                case RangeType.Protect:
                    _range = stats.ProtectRangeStopDistance;
                    break;
            }
            _rigid = transform.parent.GetComponent<Rigidbody2D>();
            _transform = transform.parent;
            _blackboard = blackboard;
            _animator = animator;
            _stats = stats;
        }
        
        public override NodeState Evaluate()
        {
            if (_stats.IsInAttackPhase)
            {
                State = NodeState.Failure;
                return State;
            }
            
            var player = _blackboard.GetData<Transform>(_stats.PlayerTag);
            var position = _transform.position;
            var distance = Vector2.Distance(position, player.position);
            var direction = Vec2.Direction(position, player.position);
            var stopDistance = _stats.DetectionRadiusPlayer - _range;
            var percentage = distance / stopDistance;
            var speed = Mathf.Lerp(_currentSpeed, _stats.SpeedGoToPlayer, percentage);
            _currentSpeed = Mathf.Lerp(_currentSpeed, speed, _stats.SpeedTransition * Time.deltaTime);
            var step = _currentSpeed * Time.deltaTime;

            if (distance > stopDistance)
            {
                var targetPosition = Vector2.MoveTowards(position, player.position, step);
                _transform.position = Vector2.Lerp(position, targetPosition, _stats.PositionTransition * Time.deltaTime);
                Vec2.LookAt(_rigid, direction);

                _animator.SetBool("IsWalking", true);

                Debug.Log("Follow");
                State = NodeState.Success;
                return State;
            }

            _currentSpeed = 0;

            State = NodeState.Failure;
            return State;
        }
    }
}