using System.Collections;
using UnityEngine;

namespace PuzzleHandler
{
    public class DoorOpener :MonoBehaviour
    {
        private const float TimeTillDoorOpenerBooted = 0.9f;
        
        [SerializeField] private AudioSource _startSound;
        
        private Animator _animator;
        private bool _doorIsOpen;
        
        public delegate void DoorOpen();
        public static event DoorOpen OnDoorOpenEvent;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            DoorOpenerBootTrigger.OnBootCollision += StartAnimation;
        }
        
        private void OnDisable()
        {
            DoorOpenerBootTrigger.OnBootCollision -= StartAnimation;
        }

        private void StartAnimation()
        {
            StartCoroutine(AnimationTransition());
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("Player") && !_doorIsOpen)
            {
                OnDoorOpenEvent?.Invoke();
                _animator.Play("door_trigger_shutdown");
                _doorIsOpen = true;
            }
        }
        
        private IEnumerator AnimationTransition()
        {
            _startSound.Play();
            _animator.Play("door_trigger_start");
            yield return new WaitForSeconds(TimeTillDoorOpenerBooted);
            _animator.Play("door_trigger_idle");
        }
    }
}