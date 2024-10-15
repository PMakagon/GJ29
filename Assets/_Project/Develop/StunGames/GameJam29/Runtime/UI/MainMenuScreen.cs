using _Project.Develop.StunGames.GameJam29.Runtime.MainMenu;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Develop.StunGames.GameJam29.Runtime.UI
{
    public class MainMenuScreen : UIPanel
    {
        [SerializeField] private Button playBtn;
        [SerializeField] private Toggle showMonsterBtn;
        [SerializeField] private GameConfig gameConfig;
        private MainMenuFlow _mainMenuFlow;

        public void Init(MainMenuFlow mainMenuFlow)
        {
            _mainMenuFlow = mainMenuFlow;
            IsInitialized = true;
        }
        
        protected override void BindButtons()
        {
            playBtn.onClick.AddListener(_mainMenuFlow.Play);
            showMonsterBtn.onValueChanged.AddListener(ToggleMonsterShow);
        }

        private void ToggleMonsterShow(bool isTrue)
        {
            gameConfig.alwaysShowMonster = isTrue;
        }
        protected override void RemoveAllListeners()
        {
            playBtn.onClick.RemoveAllListeners();
        }
    }
}