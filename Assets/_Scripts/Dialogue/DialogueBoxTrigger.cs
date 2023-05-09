using UnityEngine;
using System;

namespace Dialogue
{
    public class DialogueBoxTrigger : MonoBehaviour
    {
        [SerializeField] private TextAsset _inkJson;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private bool _hasChoices = false;

        private DialogueManager _dialogueManager;
        
        public static event Action<TextAsset ,AudioSource, bool> OnDialogueBoxTriggered;

        private void Awake()
        {
            _dialogueManager = FindObjectOfType<DialogueManager>();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (_dialogueManager.IsInDialogue) return;
            if (col.CompareTag("Player") && !col.isTrigger)
            {
                OnDialogueBoxTriggered?.Invoke(_inkJson, _audioSource, _hasChoices);
                Destroy(gameObject);
            }
        }
    }
}