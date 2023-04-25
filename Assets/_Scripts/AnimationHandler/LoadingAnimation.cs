using UnityEngine;

namespace AnimationHandler
{
    public class LoadingAnimation :MonoBehaviour
    {
        private Animator _animator;
        private PlayerAnimation _state;

        private void Awake()
        {
            _animator = FindObjectOfType<LoadingAnimation>().GetComponent<Animator>();
            _state = (PlayerAnimation)Random.Range(1, 4);
        }

        private void Update()
        {
            _animator.Play($"{_state}");
        }
    }
}
