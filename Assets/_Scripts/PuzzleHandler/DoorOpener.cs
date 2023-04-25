using System.Collections;
using UnityEngine;

namespace PuzzleHandler
{
    public class DoorOpener :MonoBehaviour
    {
        public delegate void DoorOpen();
        public static event DoorOpen OnDoorOpenEvent;

        [SerializeField] private AudioSource startSound;
        
        private Animator _animator;
        
        private readonly float _timeTillDoorOpenerBooted = 0.9f;
        private bool _doorIsOpen;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            DoorOpenerBootTrigger.OnBootCollisionEvent += StartAnimation;
        }
        
        private void OnDisable()
        {
            DoorOpenerBootTrigger.OnBootCollisionEvent -= StartAnimation;
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
            startSound.Play();
            _animator.Play("door_trigger_start");
            yield return new WaitForSeconds(_timeTillDoorOpenerBooted);
            _animator.Play("door_trigger_idle");
        }
    }
}