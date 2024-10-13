using _Project.Develop.StunGames.GameJam29.Runtime.Services;
using VContainer;
using VContainer.Unity;

namespace _Project.Develop.StunGames.GameJam29.Runtime.Bootstrap
{
    public class BootstrapFlow : IStartable
    {
        private readonly SceneLoadingService _sceneLoadingService;
        private readonly UIService _uiService;
       
        [Inject]
        public BootstrapFlow(SceneLoadingService sceneLoadingService,UIService uiService)
        {
            _sceneLoadingService = sceneLoadingService;
            _uiService = uiService;
        }
        
        private void InitializeServices()
        {
            _uiService.Initialize();    
        }
        
        public async void Start()
        {
            await _sceneLoadingService.LoadUIScene();
            InitializeServices();
            ToMainMenu();
        }
        
        private async void ToMainMenu()
        {
            await _sceneLoadingService.LoadMainMenu();
        }
    }
}
