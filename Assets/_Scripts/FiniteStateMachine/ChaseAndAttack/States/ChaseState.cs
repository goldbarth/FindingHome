using BehaviorTree.Blackboard;
using FiniteStateMachine.Base;
using HelpersAndExtensions;
using NpcSettings;
using UnityEngine;

namespace FiniteStateMachine.ChaseAndAttack.States
{
    public class ChaseState : State
    {
        private readonly AudioSource _audioSource;
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly Animator _animator;
        private readonly Rigidbody2D _rigid;
        private readonly NpcData _stats;

        private Transform _lastTarget;
        private Transform _target;

        private Vector2 _velocity;

        public ChaseState(NpcData stats, Transform transform, IBlackboard blackboard, Animator animator,
            AudioSource audioSource)
        {
            _transform = transform.parent;
            _rigid = _transform.GetComponent<Rigidbody2D>();
            _audioSource = audioSource;
            _blackboard = blackboard;
            _animator = animator;
            _stats = stats;
        }
        
        protected override void OnEnter()
        {
            base.OnEnter();
            ChasePhase();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            ChasePhase();
        }

        protected override void OnExit()
        {
            base.OnExit();
            _animator.SetBool("IsWalking", false);
            _audioSource.Stop();
        }

        private void ChasePhase()
        {
            if (!IsTargetKeyInBlackboard())
                return;

            var target = GetTarget();
            if (target == null) return;

            var direction = Vec2.Direction(_transform.position, target.position);
            
            _transform.position = Vector2.SmoothDamp(_transform.position, target.position, ref _velocity,
                _stats.SmoothTimeFast);
            Vec2.LookAt(_rigid, direction);

            _animator.SetBool("IsWalking", true);
            _audioSource.PlayOneShot(_audioSource.clip);
            _stats.IsInAttackPhase = true;
        }

        private bool IsTargetKeyInBlackboard()
        {
            return _blackboard.ContainsKey(_stats.TargetTag);
        }

        private Transform GetTarget()
        {
            return _blackboard.GetData<Transform>(_stats.TargetTag);
        }
    }
}