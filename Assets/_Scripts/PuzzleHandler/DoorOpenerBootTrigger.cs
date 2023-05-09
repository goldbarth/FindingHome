using System;
using UnityEngine;

namespace PuzzleHandler
{
    public class DoorOpenerBootTrigger : MonoBehaviour
    {
        public static event Action OnBootCollision;

        private bool _booted;
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player") && !col.isTrigger && !_booted)
            {
                OnBootCollision?.Invoke();
                _booted = true;
            }
        }
    }
}