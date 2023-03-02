using System.Collections;
using AddIns;
using Ink.Runtime;
using TMPro;
using UnityEngine;

namespace Dialogue
{
    public class NpcManager : Singleton<NpcManager>
    {
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI dialogueText;
        
        [SerializeField] private AudioSource audioSource;
        
        private Controls _controls;
        private Story _currentStory;

        private readonly float _waitTillCanMove = .2f;
        private bool _onDialogueIsActive = false;

        public bool IsInDialogue { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            
            _controls = new Controls();
            _onDialogueIsActive = false;
            dialoguePanel.SetActive(false);
            IsInDialogue = false;
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
            if (_controls.UI.Submit.triggered && _onDialogueIsActive)
                ContinueDialogue();
        }

        public void EnterDialogueMode(TextAsset inkJson)
        {
            IsInDialogue = true;
            audioSource.Play();
            _currentStory = new Story(inkJson.text);
            _onDialogueIsActive = true;
            dialoguePanel.SetActive(true);
            
            ContinueDialogue();
        }
        
        private void ContinueDialogue()
        {
            if (_currentStory.canContinue)
                dialogueText.text = _currentStory.Continue();
            else
                StartCoroutine(ExitDialogueMode());
        }
        
        public bool OnDialogueActive()
        {
            return _onDialogueIsActive;
        }

        private IEnumerator ExitDialogueMode()
        {
            audioSource.Stop();
            yield return new WaitForSeconds(_waitTillCanMove);
            _onDialogueIsActive = false;
            dialoguePanel.SetActive(false);
            dialogueText.text = string.Empty;
            IsInDialogue = false;
        }
    }
}