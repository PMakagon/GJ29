using _Project.Develop.StunGames.GameJam29.Runtime.Gameplay;
using UnityEngine;

namespace _Project.Develop.StunGames.GameJam29.Runtime.UI
{
    public class HUDScreen : UIPanel
    {
        [SerializeField] private GameOverPopup gameOverPopup;
        [SerializeField] private LevelCompletedPopup levelCompletedPopup;
        
        private GameplayFlow _gameplayFlow;

        public void Init(GameplayFlow gameplayFlow)
        {
            _gameplayFlow = gameplayFlow;
            gameOverPopup.Init(_gameplayFlow);
            levelCompletedPopup.Init(_gameplayFlow);
            Subscribe();
            IsInitialized = true;
        }

        private void Subscribe()
        {
            EventHolder.onMatchStarted += StartMatch;
            EventHolder.onMatchEnded += EndMatch;
            EventHolder.onLevelExit += OnLevelExit;
        }

        private void Unsubscribe()
        {
            EventHolder.onMatchStarted -= StartMatch;
            EventHolder.onMatchEnded -= EndMatch;
            EventHolder.onLevelExit -= OnLevelExit;
        }

        private void StartMatch()
        {
            gameOverPopup.Hide();
        }

        private void OnLevelExit()
        {
            levelCompletedPopup.Show();
        }

        private void EndMatch()
        {
            gameOverPopup.Show();
        }
    }
}