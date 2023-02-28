using UnityEngine;

namespace RiddleHandler
{
    public class DoorOpenerBootTrigger : MonoBehaviour
    {
        private DoorOpener _doorOpener;

        private void Awake()
        {
            _doorOpener = FindObjectOfType<DoorOpener>();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player") && !col.isTrigger)
            {
                _doorOpener.StartAnimation();
            }
        }
    }
}