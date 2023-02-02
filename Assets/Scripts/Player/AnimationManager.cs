using UnityEngine;

namespace Player
{
    public enum AnimationState
    {
        player_idle,
        player_walk,
        player_run,
        player_jump,
        player_fall,
        player_land,
        player_attack,
        player_hit,
        player_death
    }
    
    public class AnimationManager : MonoBehaviour
    {
        public static AnimationManager Instance { get; private set; }
        private Animator _animator;
        private string _currentState;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }

            _animator = FindObjectOfType<PlayerController2D>().GetComponent<Animator>();
        }

        public void SetAnimationState(AnimationState state)
        {
            var newState = state.ToString();
            if (_currentState == newState) return;
            _animator.Play(newState);
            _currentState = newState;
        }
    }
}