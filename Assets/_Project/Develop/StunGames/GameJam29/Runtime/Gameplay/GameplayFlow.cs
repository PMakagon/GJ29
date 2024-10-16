using _Project.Develop.StunGames.GameJam29.Runtime.Services;
using _Project.Develop.StunGames.GameJam29.Runtime.UI;
using UnityEngine.Device;
using VContainer;
using VContainer.Unity;

namespace _Project.Develop.StunGames.GameJam29.Runtime.Gameplay
{
    public class GameplayFlow : IStartable
    {
        private SceneLoadingService _sceneLoadingService;
        private MatchController _matchController;
        private UIService _uiService;


        [Inject]
        public GameplayFlow(SceneLoadingService sceneLoadingService, MatchController matchController,
            UIService uiService)
        {
            _sceneLoadingService = sceneLoadingService;
            _matchController = matchController;
            _uiService = uiService;
        }

        public void Start()
        {
            _matchController.Initialize();
            _uiService.InitializeHUDScreen(this);
            _uiService.ShowUIPanel<HUDScreen>();
            StartGame();
        }

        public void StartGame()
        {
            _matchController.StartMatch();
        }

        public void GameOver()
        {
            RestartGame();
        }

        public async void RestartGame()
        {
            _matchController.IsFirstMatch = true;
            StartGame();
        }

        public async void StartNextlevel()
        {
            _matchController.IsFirstMatch = false;
            StartGame();
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}