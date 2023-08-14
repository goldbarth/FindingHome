using FiniteStateMachine.FollowPlayer.States;
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
            EatEdibleState.OnConsumeEdible += TriggerSpitterDialogue;
        }

        private void OnDisable()
        {
            EatEdibleState.OnConsumeEdible -= TriggerSpitterDialogue;
        }

        private void TriggerSpitterDialogue()
        {
            OnEatDialogueTriggered?.Invoke(_inkJson, _audioSource, _hasChoices);
        }
    }
}
