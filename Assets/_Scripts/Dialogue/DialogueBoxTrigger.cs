﻿using UnityEngine;

namespace Dialogue
{
    public class DialogueBoxTrigger : MonoBehaviour
    {
        [SerializeField] private TextAsset _inkJson;

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (CharMemoryManager.Instance.OnDialogueActive()) return;
            if (col.CompareTag("Player") && !col.isTrigger)
                CharMemoryManager.Instance.EnterDialogueMode(_inkJson);
            Destroy(gameObject);
        }
    }
}