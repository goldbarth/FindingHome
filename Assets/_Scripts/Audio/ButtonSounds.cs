using UnityEngine;

namespace Audio
{
    public class ButtonSounds : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;

        public void ButtonHoverSound()
        {
            _audioSource.Play();
        }
    }
}
