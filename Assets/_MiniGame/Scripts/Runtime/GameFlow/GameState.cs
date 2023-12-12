using System;

namespace UrbanFox.MiniGame
{
    [Serializable]
    public enum GameState
    {
        Loading,
        SplashScreen,
        GameplayPausable,
        GameplayNonPausable,
        GameOverWaitForReload,
        Paused
    }
}
