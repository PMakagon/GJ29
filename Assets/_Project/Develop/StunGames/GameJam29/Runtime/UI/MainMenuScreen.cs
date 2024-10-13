using _Project.Develop.StunGames.GameJam29.Runtime.MainMenu;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Develop.StunGames.GameJam29.Runtime.UI
{
    public class MainMenuScreen : UIPanel
    {
        [SerializeField] private Button playBtn;
        private MainMenuFlow _mainMenuFlow;

        public void Init(MainMenuFlow mainMenuFlow)
        {
            _mainMenuFlow = mainMenuFlow;
            IsInitialized = true;
        }
        
        protected override void BindButtons()
        {
            playBtn.onClick.AddListener(_mainMenuFlow.Play);
        }
        protected override void RemoveAllListeners()
        {
            playBtn.onClick.RemoveAllListeners();
        }
    }
}