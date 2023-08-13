using HelpersAndExtensions;

public class GameManager : Singleton<GameManager>
{
    public bool IsGamePaused { get; set; }
    public bool OnRoomReset { get; set; }
    public bool IsNewGame { get; set; }
    public bool IsMenuActive { get; set; }
    public bool IsPauseMenuActive { get; set; }
    public bool OnApplicationStart { get; set; }
    public bool IsSelected { get; set; }
    public bool IsGameStarted { get; set; }
    public bool IsRespawning { get; set; }

    protected override void Awake()
    {
        base.Awake();
        
        OnApplicationStart = true;
    }
}