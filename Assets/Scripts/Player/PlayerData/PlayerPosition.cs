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
            data.playerPosition = transform.position;
        }
    }
}
