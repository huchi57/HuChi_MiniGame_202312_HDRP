using System;
using UnityEngine;

namespace UrbanFox.MiniGame
{
    public static class GameInstance
    {
        public const string PlayerTag = "Player";
        public static event Action<GameState> OnGameStateChanged;

        public static GameState CurrentGameState
        {
            get;
            private set;
        }

        public static PlayerController PlayerController
        {
            get;
            private set;
        }

        public static Transform PlayerTransform
        {
            get;
            private set;
        }

        private static GameState m_previousGameState;

        public static void SwitchGameState(GameState state)
        {
            m_previousGameState = CurrentGameState;
            CurrentGameState = state;
            OnGameStateChanged?.Invoke(state);
        }

        public static void SwitchToPreviousGameState()
        {
            SwitchGameState(m_previousGameState);
        }

        public static void RegisterPlayer(PlayerController playerController)
        {
            PlayerController = playerController;
            if (playerController)
            {
                PlayerTransform = playerController.transform;
            }
        }
    }
}
