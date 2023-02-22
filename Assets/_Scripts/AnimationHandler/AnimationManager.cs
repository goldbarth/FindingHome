using AddIns;
using Player;
using UnityEngine;

namespace AnimationHandler
{
    public enum PlayerAnimationState
    {
        player_idle,
        player_walk,
        player_run,
        player_dash,
        player_jump,
        player_fall,
        player_land,
        player_wallclimb,
        player_wallslide,
        player_wallgrab,
        player_attack,
        player_hit,
        player_death
    }
    
    public class AnimationManager : Singleton<AnimationManager>
    {
        private Animator _animator;
        private string _currentState;

        protected override void Awake()
        {
            base.Awake();
            _animator = FindObjectOfType<PlayerController2D>().GetComponentInChildren<Animator>();
        }

        public void SetAnimationState(PlayerAnimationState state)
        {
            var newState = $"{state}";
            if (_currentState == newState) return;
            _animator.Play(newState);
            _currentState = newState;
        }
    }
}