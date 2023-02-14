using AddIns;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public bool IsPaused { get; set; }
    public bool IsLoadingGame { get; set; }
    public bool IsNewGame { get; set; }

    protected override void Awake()
    {
        base.Awake();
    }
}