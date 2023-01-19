namespace DataPersistence
{
    public interface IDataPersistence
    {
        void LoadData(ref GameData data);
        void SaveData(ref GameData data);
    }
}
