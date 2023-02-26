using DataPersistence;
using UnityEngine;

namespace Player.PlayerData
{
    public class PlayerPosition : MonoBehaviour, IDataPersistence
    {
        public void LoadData(GameData data)
        {
            transform.position = data.playerPosition;
        }

        public void SaveData(GameData data)
        {
            // prevents saving the player position when not changing the room
            if (!GameManager.Instance.OnRoomReset)
            {
                data.playerPosition = transform.position;
            }
        }
    }
}
