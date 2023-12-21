using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

namespace Audio
{
    public class ButtonSounds : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;

        public void ButtonHoverSound()
        {
            if (IsButtonInteractable())
                _audioSource.Play();
        }
        
        private bool IsButtonInteractable()
        {
            var currentButton = EventSystem.current.currentSelectedGameObject;
            return currentButton != null && currentButton.GetComponent<Button>().interactable;
        }
    }
}
