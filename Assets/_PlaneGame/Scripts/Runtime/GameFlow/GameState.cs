using System;

namespace UrbanFox.MiniGame
{
    [Serializable]
    public enum GameState
    {
        Loading,
        WaitForInputToStartGame,
        GameplayPausable,
        GameplayNonPausable,
        GameOverWaitForReload,
        Paused,
        GameCompletedWaitForInput
    }
}
