using BehaviorTree.Blackboard;
using FiniteStateMachine.Base;
using HelpersAndExtensions;
using NpcSettings;
using UnityEngine;
using System;

namespace FiniteStateMachine.FollowPlayer.States
{
    public class EatEdibleState : State
    {
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly Rigidbody2D _rigid;
        private readonly Animator _animator;
        private readonly NpcData _stats;
        private readonly float _timer;
        
        private Transform _player;
        private Vector3 _position;
        private Vector2 _velocity;
        private float _distance;

        public static event Action OnConsumeEdible;

        public EatEdibleState(NpcData stats, Transform transform, Animator animator, IBlackboard blackboard)
        {
            _transform = transform.parent.GetComponent<Transform>();
            _rigid = transform.parent.GetComponent<Rigidbody2D>();
            _position = _transform.position;
            _blackboard = blackboard;
            _animator = animator;
            _stats = stats;
        }
        
        protected override void OnUpdate()
        {
            base.OnUpdate();
            EatPhase();
        }

        private void EatPhase()
        {
            _player = GetTarget();
            if (_player == null) return;
            
            _position = _transform.position;
            _distance = GetDistance();
            _transform.position = CalculateSmoothDamp();
            
            SetLookDirection();

            if (IsDistanceLessThanStopDistance())
            {
                _rigid.velocity = Vector2.zero;
                OnConsumeEdible?.Invoke();
                _animator.SetTrigger("IsEatingTrigger");
                
                StateController.EatEdibleState.OnExit();
            }
        }

        private float GetDistance()
        {
            return Vector2.Distance(_position, _player.position);
        }

        private Vector2 CalculateSmoothDamp()
        {
            return Vector2.SmoothDamp(_position, _player.position, ref _velocity, _stats.SmoothTimeFast);
        }

        private void SetLookDirection()
        {
            var direction = GetDirection();
            Vec2.LookAt(_rigid, direction);
        }

        private Vector2 GetDirection()
        {
            return Vec2.Direction(_position, _player.position);
        }

        private bool IsDistanceLessThanStopDistance()
        {
            return _distance <= _stats.StopDistanceEat;
        }

        private Transform GetTarget()
        {
            return _blackboard.GetData<Transform>(_stats.PlayerTag);
        }
    }
}