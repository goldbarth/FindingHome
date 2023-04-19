using System.Collections;
using Environment;
using UnityEngine;

namespace AnimationHandler
{
    public class SaveAnimation : MonoBehaviour
    {
        [Header("SAVE CANVAS")]
        [SerializeField] private GameObject _saveCanvas;
        [SerializeField] private Animator _saveCanvasAnimator;

        private const float SAVE_ANIMATION_TIME = 1.7f;

        private void OnDisable()
        {
            Room.OnRoomEnterEvent -= Play;
        }
        
        private void OnEnable()
        {
            Room.OnRoomEnterEvent += Play;
        }

        private void Play()
        {
            StartCoroutine(PlaySaveAnimation());
        }
        
        private IEnumerator PlaySaveAnimation()
        {
            _saveCanvas.SetActive(true);
            _saveCanvasAnimator.Play("save_text");
            yield return new WaitForSeconds(SAVE_ANIMATION_TIME);
            _saveCanvas.SetActive(false);
        }
    }
}