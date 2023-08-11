using BehaviorTree.Blackboard;
using BehaviorTree.NPCStats;
using BehaviorTree.Core;
using UnityEngine;
using AddIns;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionChaseTarget : ActionNode
    {
        private readonly AudioSource _audioSource;
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly SpitterStats _stats;
        private readonly Animator _animator;
        private readonly Rigidbody2D _rigid;
        
        private Vector2 _velocity;
        private float _currentSpeed;

        public ActionChaseTarget(SpitterStats stats, Transform transform, Animator animator, IBlackboard blackboard, AudioSource audioSource)
        {
            _rigid = transform.parent.GetComponent<Rigidbody2D>();
            _transform = transform.parent;
            _audioSource = audioSource;
            _blackboard = blackboard;
            _animator = animator;
            _stats = stats;
        }
        
        public override NodeState Evaluate()
        {
            if(!_blackboard.ContainsKey(_stats.TargetTag))
            {
                State = NodeState.Failure;
                return State;
            }
            
            var target = _blackboard.GetData<Transform>(_stats.TargetTag);
            if(target == null)
            {
                State = NodeState.Failure;
                return State;
            }
            
            var direction = Vec2.Direction(_transform.position, target.position);
            var distance = Vector2.Distance(_transform.position, target.position);
            
            if (distance > _stats.TargetStopDistance)
            {
                _transform.position = Vector2.SmoothDamp(_transform.position, target.position, ref _velocity, _stats.SmoothTimeFast);
                Vec2.LookAt(_rigid, direction);

                _animator.SetBool("IsWalking", true);
                _audioSource.PlayOneShot(_audioSource.clip);
                
                State = NodeState.Running;
                return State; 
            }

            if (distance < _stats.TargetStopDistance)
            {
                _animator.SetBool("IsWalking", false);
                _audioSource.Stop();
                
                State = NodeState.Success;
                return State;
            }
            
            State = NodeState.Failure;
            return State;
        }
    }
}