using System;
using UnityEngine;

namespace UrbanFox.MiniGame
{
    public static class GameStateManager
    {
        public static event Action<GameState> OnGameStateChanged;

        public static GameState CurrentGameState
        {
            get;
            private set;
        }

        public static PlayerController Player
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

        public static void SwitchGameState(GameState newState)
        {
            m_previousGameState = CurrentGameState;
            CurrentGameState = newState;
            OnGameStateChanged?.Invoke(CurrentGameState);
        }

        public static void SwitchToPreviousGameState()
        {
            SwitchGameState(m_previousGameState);
        }

        public static void RegisterPlayer(PlayerController player)
        {
            Player = player;
            if (player)
            {
                PlayerTransform = player.transform;
            }
        }
    }
}
