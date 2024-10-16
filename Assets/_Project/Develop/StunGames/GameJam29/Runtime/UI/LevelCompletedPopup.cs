using _Project.Develop.StunGames.GameJam29.Runtime.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Develop.StunGames.GameJam29.Runtime.UI
{
    public class LevelCompletedPopup : UIPanel
    {
        [SerializeField] private Button restartBtn;
        [SerializeField] private Button exitBtn;

        private GameplayFlow _gameplayFlow;
        
        public void Init(GameplayFlow gameplayFlow)
        {
            _gameplayFlow = gameplayFlow;
            IsInitialized = true;
        }

        protected override void BindButtons()
        {
            restartBtn.onClick.AddListener(_gameplayFlow.StartNextlevel);
            restartBtn.onClick.AddListener(Hide);
            exitBtn.onClick.AddListener(_gameplayFlow.GameOver);
            exitBtn.onClick.AddListener(Hide);
        }

        protected override void RemoveAllListeners()
        {
            restartBtn.onClick.RemoveAllListeners();
            exitBtn.onClick.RemoveAllListeners();
        }
    }
}