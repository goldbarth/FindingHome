using AddIns;
using DataPersistence;

namespace Player.PlayerData
{
    public class DeathCount : Singleton<DeathCount>, IDataPersistence
    {
        private int _deathCount = 0;
        
        public void OnPlayerDeath()
        {
            _deathCount++;
        }

        public void LoadData(GameData data)
        {
            _deathCount = data.deathCount;
        }

        public void SaveData(GameData data)
        {
            data.deathCount = _deathCount;
        }
    }
}
