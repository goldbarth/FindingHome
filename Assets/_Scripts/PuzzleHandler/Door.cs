using System;
using DataPersistence;
using UnityEngine;

namespace PuzzleHandler
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private AudioSource _openSound;
        
        private Collider2D _collider;
        private Animator _animator;

        private void Start()
        {
            _collider = GetComponent<Collider2D>();
            _animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            DoorOpener.OnDoorOpenEvent += Open;
        }
        
        private void OnDisable()
        {
            DoorOpener.OnDoorOpenEvent -= Open;
        }
        
        private void Open(bool isOpen)
        {
            if (!isOpen)
                _openSound.Play();
            
            _collider.enabled = false;
            _animator.Play("door_open");
            
        }
    }
}