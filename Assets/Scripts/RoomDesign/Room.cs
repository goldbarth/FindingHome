using System;
using UnityEngine;

namespace RoomDesign
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private GameObject virtualCamera;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !other.isTrigger)
            {
                virtualCamera.SetActive(true);
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !other.isTrigger)
            {
                virtualCamera.SetActive(false);
            }
        }
    }
}
