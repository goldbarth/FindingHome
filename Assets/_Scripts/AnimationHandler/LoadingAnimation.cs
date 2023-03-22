using UnityEngine;

namespace AnimationHandler
{
    internal enum PlayerAnimation
    {
        player_idle,
        player_walk,
        player_run,
        player_dash
    }
    public class LoadingAnimation :MonoBehaviour
    {
        private Animator _animator;
        private PlayerAnimation _state;

        private void Awake()
        {
            _animator = FindObjectOfType<LoadingAnimation>().GetComponent<Animator>();
            _state = (PlayerAnimation)Random.Range(1, 5);
        }

        private void Update()
        {
            _animator.Play($"{_state}");
        }
    }
}
