using System.Collections;
using UnityEngine;

namespace RiddleHandler
{
    public class DoorOpener :MonoBehaviour
    {
        private Door _door;
        private Animator _animator;
        
        private readonly float _timeTillDoorOpenerBooted = 1f;

        private void Start()
        {
            _door = FindObjectOfType<Door>();
            _animator = GetComponent<Animator>();
        }

        public void StartAnimation()
        {
            StartCoroutine(AnimationTransition());
        }

        private IEnumerator AnimationTransition()
        {
            _animator.Play("door_trigger_start");
            yield return new WaitForSeconds(_timeTillDoorOpenerBooted);
            _animator.Play("door_trigger_idle");
        }

        private bool _doorIsOpen;
        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("Player") && !_doorIsOpen)
            {
                _door.Open();
                _animator.Play("door_trigger_shutdown");
                _doorIsOpen = true;
            }
        }
    }
}