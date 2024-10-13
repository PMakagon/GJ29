using _Project.Develop.StunGames.GameJam29.Runtime.Services;
using _Project.Develop.StunGames.GameJam29.Runtime.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Project.Develop.StunGames.GameJam29.Runtime.MainMenu
{
    public class MainMenuFlow : IStartable
    {
        private readonly SceneLoadingService _sceneLoadingService;
        private readonly UIService _uiService;

        [Inject]
        public MainMenuFlow(SceneLoadingService sceneLoadingService, UIService uiService)
        {
            _sceneLoadingService = sceneLoadingService;
            _uiService = uiService;
        }

        public void Start()
        {
            _uiService.InitializeMainMenuScreen(this);
            _uiService.ShowUIPanel<MainMenuScreen>();
        }
        
        public async void Play()
        {
            await _sceneLoadingService.UnloadMainMenu();
            await _sceneLoadingService.LoadGameplayScene();
            _uiService.HideUIPanel<MainMenuScreen>();
        }
    }
}