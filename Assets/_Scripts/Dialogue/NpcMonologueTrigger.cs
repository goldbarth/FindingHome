using UnityEngine;

namespace Dialogue
{
    public class NpcMonologueTrigger : MonoBehaviour
    {
        [SerializeField] private TextAsset inkJson;

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (NpcManager.Instance.OnDialogueActive()) return;
            if (col.CompareTag("Player") && !col.isTrigger)
                NpcManager.Instance.EnterDialogueMode(inkJson);
            Destroy(gameObject);
        }
    }
}