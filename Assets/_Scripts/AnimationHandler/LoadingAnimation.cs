using UnityEngine;

namespace AnimationHandler
{
    public class LoadingAnimation :MonoBehaviour
    {
        private Animator _animator;
        private PlayerAnimationState _state;

        private void Awake()
        {
            _animator = FindObjectOfType<LoadingAnimation>().GetComponent<Animator>();
            _state = (PlayerAnimationState)Random.Range(1, 6);
        }

        private void Update()
        {
            _animator.Play($"{_state}");
        }
    }
}
