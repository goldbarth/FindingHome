using BehaviorTree.Blackboard;
using BehaviorTree.NPCStats;
using BehaviorTree.Core;
using UnityEngine;
using AddIns;

namespace BehaviorTree.Nodes.Actions
{
    public class ActionProtectPlayer : ActionNode
    {
        private readonly IBlackboard _blackboard;
        private readonly Transform _transform;
        private readonly SpitterStats _stats;
        private readonly Animator _animator;
        private readonly Rigidbody2D _rigid;

        public ActionProtectPlayer(SpitterStats stats, Transform transform, Animator animator, IBlackboard blackboard)
        {
            _rigid = transform.parent.GetComponent<Rigidbody2D>();
            _transform = transform.parent;
            _blackboard = blackboard;
            _animator = animator;
            _stats = stats;
        }
        
        public override NodeState Evaluate()
        {
            if (_stats._isInAttackPhase)
            {
                State = NodeState.Failure;
                return State;
            }
            
            var player = _blackboard.GetData<Transform>("player");
            var distance = Vector2.Distance(_transform.position, player.position);
            var direction = ((Vector2)player.position - (Vector2)_rigid.transform.position).normalized;
            var step = _stats._speedPlayerFollow * Time.deltaTime;
            if (distance > _stats._stopDistancePlayerProtect)
            {
                _animator.SetBool("IsWalking", true);
                Vec2.MoveTo(_transform, player, step);
                Vec2.LookAt(_rigid, direction);
            }
            else
            {
                _animator.SetBool("IsWalking", false);
            }

            State = NodeState.Success;
            return State;
        }
    }
}