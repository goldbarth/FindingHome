using UnityEngine;
using System;
using DataPersistence;
using Player;

namespace Dialogue
{
    public class DialogueBoxTrigger : MonoBehaviour, IDataPersistence
    {
        [SerializeField] private TextAsset _inkJson;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private bool _hasChoices = false;
        [SerializeField] private bool _activatesMultiJump = false;

        private DialogueManager _dialogueManager;
        private bool _isDoorOpen;
        
        public static event Action<TextAsset ,AudioSource, bool> OnDialogueBoxTriggered;

        private void Awake()
        {
            _dialogueManager = FindObjectOfType<DialogueManager>();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (_dialogueManager.IsInDialogue || _isDoorOpen) return;
            if (col.CompareTag("Player") && !col.isTrigger)
            {
                OnDialogueBoxTriggered?.Invoke(_inkJson, _audioSource, _hasChoices);
                if (_activatesMultiJump)
                    col.GetComponent<PlayerController>().ActivateMultiJump();
                Destroy(gameObject);
            }
        }

        public void LoadData(GameData data)
        {
            _isDoorOpen = data.isDoorOpen;
        }

        public void SaveData(GameData data)
        { }
    }
}