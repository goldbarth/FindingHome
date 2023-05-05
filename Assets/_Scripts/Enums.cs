public enum NodeState
{
    Success,
    Failure,
    Running
}

public enum SpitterAnimationEvents
{
    ChangeController,
}


public enum CollectableType
{
    Eatable,
    Can
}

public enum SceneIndices
{
    Init,
    MainMenu,
    OptionsMenu,
    PauseMenu,
    LoadMenu,
    CreditsScreen,
    Level1,
    Level2,
}

// these has to be against naming conventions,
// because animation names are names like this
// and enums are used to get the animation names
public enum PlayerAnimation
{
    player_idle,
    player_walk,
    player_run,
    player_dash
}
