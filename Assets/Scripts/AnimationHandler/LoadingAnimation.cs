using UnityEngine;

namespace AnimationHandler
{
    public class LoadingAnimation :MonoBehaviour
    {
        private Animator _animator;
        private AnimationState _state;

        private void Awake()
        {
            _animator = FindObjectOfType<LoadingAnimation>().GetComponent<Animator>();
            _state = (AnimationState)Random.Range(0, 4);
        }
        
        private void Update()
        {
            _animator.Play(_state.ToString());
        }
    }
}
