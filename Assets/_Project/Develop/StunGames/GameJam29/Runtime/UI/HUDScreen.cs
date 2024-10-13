using _Project.Develop.StunGames.GameJam29.Runtime.Gameplay;
using UnityEngine;

namespace _Project.Develop.StunGames.GameJam29.Runtime.UI
{
    public class HUDScreen : UIPanel
    {
        [SerializeField] private GameOverPopup gameOverPopup;
        
        private GameplayFlow _gameplayFlow;

        public void Init(GameplayFlow gameplayFlow)
        {
            _gameplayFlow = gameplayFlow;
            gameOverPopup.Init(_gameplayFlow);
            Subscribe();
            IsInitialized = true;
        }

        private void Subscribe()
        {
            EventHolder.MatchStarted += StartMatch;
            EventHolder.MatchEnded += EndMatch;
        }

        private void Unsubscribe()
        {
            EventHolder.MatchStarted -= StartMatch;
            EventHolder.MatchEnded -= EndMatch;
        }

        private void StartMatch()
        {
            gameOverPopup.Hide();
        }

        private void EndMatch()
        {
            gameOverPopup.Show();
        }
    }
}