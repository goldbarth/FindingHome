using UnityEngine;

namespace RiddleHandler
{
    public class Door : MonoBehaviour
    {
        private Collider2D _collider;
        private Animator _animator;

        private void Start()
        {
            _collider = GetComponent<Collider2D>();
            _animator = GetComponent<Animator>();
        }

        public void Open()
        {
            _collider.enabled = false;
            _animator.Play("door_open");
            Debug.Log("Door opened");
        }
    }
}