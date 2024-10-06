using _Project.Develop.StunGames.GameJam29.Runtime.Services;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Project.Develop.StunGames.EmptyProject.Runtime.Bootstrap
{
    public class BootstrapFlow : IStartable
    {
        private readonly SceneLoaderService _sceneLoaderService;
       
        [Inject]
        public BootstrapFlow(SceneLoaderService sceneLoaderService)
        {
            _sceneLoaderService = sceneLoaderService;
        }
        
        public void Start()
        {
            Debug.Log("BOOTSTRAP START");
            ToGame();
        }
        
        private void ToGame()
        {
            _sceneLoaderService.LoadSceneByName(SceneLoaderService.UI_SCENE);
            _sceneLoaderService.LoadSceneByName(SceneLoaderService.CORE_SCENE);
        }
    }
}
