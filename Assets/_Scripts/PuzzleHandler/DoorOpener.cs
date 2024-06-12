using System.Collections;
using DataPersistence;
using UnityEngine;

namespace PuzzleHandler
{
    public class DoorOpener :MonoBehaviour, IDataPersistence
    {
        private const float TimeTillDoorOpenerBooted = 0.9f;
        
        [SerializeField] private AudioSource _startSound;
        
        private Animator _animator;
        private bool _isDoorOpen;
        
        public delegate void DoorOpen(bool isOpen);
        public static event DoorOpen OnDoorOpenEvent;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            
            if (_isDoorOpen)
                OnDoorOpenEvent?.Invoke(true);
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
            if (col.gameObject.CompareTag("Player") && !_isDoorOpen)
            {
                OnDoorOpenEvent?.Invoke(false);
                _animator.Play("door_trigger_shutdown");
                _isDoorOpen = true;
            }
        }
        
        private IEnumerator AnimationTransition()
        {
            _startSound.Play();
            _animator.Play("door_trigger_start");
            yield return new WaitForSeconds(TimeTillDoorOpenerBooted);
            _animator.Play("door_trigger_idle");
        }

        public void LoadData(GameData data)
        {
            _isDoorOpen = data.isDoorOpen;
        }

        public void SaveData(GameData data)
        {
            data.isDoorOpen = _isDoorOpen;
        }
    }
}