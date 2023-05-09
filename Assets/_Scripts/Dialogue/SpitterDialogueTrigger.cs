using BehaviorTree.Nodes.Actions;
using UnityEngine;
using System;

namespace Dialogue
{
    public class SpitterDialogueTrigger : MonoBehaviour
    {
        [SerializeField] private TextAsset _inkJson;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private bool _hasChoices = false;

        public static event Action<TextAsset ,AudioSource, bool> OnEatDialogueTriggered;

        private void OnEnable()
        {
            ActionConsumeEatable.OnConsumeEatable += TriggerSpitterDialogue;
        }

        private void OnDisable()
        {
            ActionConsumeEatable.OnConsumeEatable -= TriggerSpitterDialogue;
        }

        private void TriggerSpitterDialogue()
        {
            OnEatDialogueTriggered?.Invoke(_inkJson, _audioSource, _hasChoices);
        }
    }
}
