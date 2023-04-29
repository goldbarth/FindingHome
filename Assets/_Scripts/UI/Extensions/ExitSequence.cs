using System.Collections;
using AnimationHandler;
using SceneHandler;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.Extensions
{
    public class ExitSequence : MonoBehaviour
    {
        [SerializeField] private GameObject _exitCanvas;
        [SerializeField] private GameObject _teleportAnimation;
        [SerializeField] private GameObject _walkAnimation;
        [SerializeField] private GameObject _closingWords;
        [SerializeField] private GameObject _menuButton;
        [SerializeField] private AudioSource _stepAudio;
        [SerializeField] private AudioSource _teleportAudio;

        private const float WaitToPitch = 4.5f;
        private const float StopAudio = 1.5f;
        private const float DisplayEndScreen = 2f;

        private void OnEnable()
        {
            LevelExitAnimation.OnLevelExitEvent += StartExitSequence;
        }

        private void OnDisable()
        {
            LevelExitAnimation.OnLevelExitEvent -= StartExitSequence;
        }
        
        private void StartExitSequence()
        {
            _exitCanvas.SetActive(true);
            StartCoroutine(CharExitSequence());
        }

        private IEnumerator CharExitSequence()
        {
            _teleportAudio.Play();
            _teleportAnimation.SetActive(true);
            yield return new WaitForSeconds(.8f);
            _teleportAnimation.SetActive(false);
            _walkAnimation.SetActive(true);
            _teleportAudio.Stop();
            _stepAudio.enabled = true;
            yield return new WaitForSeconds(WaitToPitch);
            _stepAudio.pitch = 1f;
            yield return new WaitForSeconds(StopAudio);
            _stepAudio.Stop();
            _walkAnimation.SetActive(false);
            yield return new WaitForSeconds(DisplayEndScreen);
            _closingWords.SetActive(true);
            _menuButton.gameObject.SetActive(true);
        }
    }
}