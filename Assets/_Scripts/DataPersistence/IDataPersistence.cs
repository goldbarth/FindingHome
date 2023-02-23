namespace DataPersistence
{
    // this is a super handy interface that allows to
    // save and load data from any class that implements it
    public interface IDataPersistence
    {
        void LoadData(GameData data);
        void SaveData(GameData data);
    }
}