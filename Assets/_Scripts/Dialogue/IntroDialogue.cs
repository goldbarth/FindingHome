using UnityEngine;
using System;

namespace Dialogue
{
    public class IntroDialogue : MonoBehaviour
    {
        [SerializeField] private GameObject _popup;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private TextAsset _inkJson;
        [SerializeField] private bool _hasChoices = true;
        
        private DialogueManager _dialogueManager;
        private Controls _controls;
        private bool _inRange;
        
        public static event Action<TextAsset, AudioSource, bool> OnDialogueIntro;

        private void Awake()
        {
            _dialogueManager = FindObjectOfType<DialogueManager>();
            _controls = new Controls();
            _popup.SetActive(false);
            _inRange = false;
        }

        private void OnEnable()
        {
            _controls.Enable();
        }

        private void OnDisable()
        {
            _controls.Disable();
        }

        private void Update()
        {
            _popup.SetActive(_inRange);
            if (_inRange && !_dialogueManager.IsInDialogue)
                if (_controls.Gameplay.Interact.triggered)
                    OnDialogueIntro?.Invoke(_inkJson, _audioSource, _hasChoices);
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if(col.CompareTag("Player") && !col.isTrigger)
                _inRange = true;
            
            var hitPos = col.ClosestPoint(transform.position);
            _spriteRenderer.flipX = hitPos.x < transform.position.x;
        }
        
        private void OnTriggerExit2D(Collider2D col)
        {
            if(col.CompareTag("Player") && !col.isTrigger)
                _inRange = false;
            
            var hitPos = col.ClosestPoint(transform.position);
            _spriteRenderer.flipX = hitPos.x < transform.position.x;
        }
    }
}