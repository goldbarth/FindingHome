using UnityEngine;

//TODO: test more or delete!!!
namespace AnimationHandler
{
    public class LoadingAnimation : MonoBehaviour
    {
        private Animator _animator;
        private string _currentState;
        private AnimationState _state;

        private void Awake()
        {
            _animator = FindObjectOfType<LoadingAnimation>().GetComponent<Animator>();
            _state = (AnimationState)Random.Range(1, 3);
        }
        
        private void Update()
        {
            SetAnimationState(_state);
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
