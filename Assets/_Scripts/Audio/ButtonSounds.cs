using UnityEngine;

namespace Audio
{
    public class ButtonSounds : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;

        public void ButtonHoverSound()
        {
            audioSource.Play();
        }
    }
}
