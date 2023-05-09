using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine;
using Ink.Runtime;
using TMPro;

namespace Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        private const float WaitTillCanMove = .2f;
        
        [SerializeField] private GameObject _dialoguePanel;
        [SerializeField] private TextMeshProUGUI _dialogueText;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private bool _hasChoices = false;
        [SerializeField] private GameObject[] _choices;
        
        private TextMeshProUGUI[] _choiceTexts;
        private Story _currentStory;
        private Controls _controls;

        public bool IsInDialogue { get; private set; } = false;

        private void Awake()
        {
            _choiceTexts = new TextMeshProUGUI[_choices.Length];
            _dialoguePanel.SetActive(false);
            _controls = new Controls();
            IsInDialogue = false;
            
            var index = 0;
            foreach (var choice in _choices)
            {
                _choiceTexts[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
                index++;
            }
        }

        private void OnEnable()
        {
            IntroDialogue.OnDialogueIntro += EnterDialogueMode;
            DialogueBoxTrigger.OnDialogueBoxTriggered += EnterDialogueMode;
            SpitterDialogueTrigger.OnEatDialogueTriggered += EnterDialogueMode;
            _controls.Enable();
        }
        
        private void OnDisable()
        {
            IntroDialogue.OnDialogueIntro -= EnterDialogueMode;
            DialogueBoxTrigger.OnDialogueBoxTriggered -= EnterDialogueMode;
            SpitterDialogueTrigger.OnEatDialogueTriggered -= EnterDialogueMode;
            _controls.Disable();
        }

        private void Update()
        {
            if (_controls.UI.Submit.triggered && IsInDialogue)
                ContinueDialogue();
        }


        private void EnterDialogueMode(TextAsset inkJson, AudioSource audioSource, bool hasChoices = false)
        {
            _audioSource = audioSource;
            _hasChoices = hasChoices;
            IsInDialogue = true;
            _audioSource.Play();
            
            _currentStory = new Story(inkJson.text);
            _dialoguePanel.SetActive(true);
            
            ContinueDialogue();
        }

        private void ContinueDialogue()
        {
            if (_currentStory.canContinue)
            {
                _dialogueText.text = _currentStory.Continue();
                if(_hasChoices)
                    DisplayChoices();
            }
            else
                StartCoroutine(ExitDialogueMode());
        }

        private void DisplayChoices()
        {
            var currentChoices = _currentStory.currentChoices;

            if (currentChoices.Count > _choices.Length)
                Debug.LogError($"Not enough choices in the UI. Number of choices in the UI: " +
                               $"{_choices.Length} Number of choices in the ink file: {currentChoices.Count}");

            // enable and initialize the choices in the UI
            var index = 0;
            foreach (var choice in currentChoices)
            {
                _choices[index].gameObject.SetActive(true);
                _choiceTexts[index].text = choice.text;
                index++;
            }
            
            // disable the rest of the choices
            for (var i = index; i < _choices.Length; i++)
                _choices[i].gameObject.SetActive(false);
            
            StartCoroutine(SelectChoice());
        }
        
        public void ChooseChoice(int choiceIndex)
        {
            _currentStory.ChooseChoiceIndex(choiceIndex);
        }

        private IEnumerator SelectChoice()
        {
            EventSystem.current.SetSelectedGameObject(null);
            yield return new WaitForEndOfFrame();
            EventSystem.current.SetSelectedGameObject(_choices[0].gameObject);
        }
        
        private IEnumerator ExitDialogueMode()
        {
            _audioSource.Stop();
            yield return new WaitForSeconds(WaitTillCanMove);
            _dialoguePanel.SetActive(false);
            _dialogueText.text = string.Empty;
            IsInDialogue = false;
        }
    }
}