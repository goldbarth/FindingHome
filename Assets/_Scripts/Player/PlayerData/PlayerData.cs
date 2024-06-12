using DataPersistence;
using UnityEngine;

namespace Player.PlayerData
{
    public class PlayerData : MonoBehaviour, IDataPersistence
    {
        public void LoadData(GameData data)
        {
            transform.position = data.playerPosition;
            if (data.isMultiJumpActive)
                GetComponent<PlayerController>().ActivateMultiJump();
            
        }

        public void SaveData(GameData data)
        {
            if (GameManager.Instance.IsGameStarted && !GameManager.Instance.IsGamePaused)
            {
                data.playerPosition = transform.position;
                data.isMultiJumpActive = GetComponent<PlayerController>().IsMultiJumpActive();
            }
        }
    }
}