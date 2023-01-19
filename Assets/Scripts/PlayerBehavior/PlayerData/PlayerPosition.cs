using DataPersistence;
using UnityEngine;

namespace PlayerBehavior.PlayerData
{
    public class PlayerPosition : MonoBehaviour, IDataPersistence
    {
        public void LoadData(ref GameData data)
        {
            transform.position = data.playerPosition;
        }

        public void SaveData(ref GameData data)
        {
            data.playerPosition = transform.position;
        }
    }
}
