using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Develop.StunGames.GameJam29.Runtime.Services
{
    public class SceneLoadingService
    {
        private const string UI = "UI";
        private const string MAIN_MENU = "MAIN_MENU";
        private const string GAMEPLAY = "GAMEPLAY";
        
        private void LoadSceneByName(string sceneName)
        {
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }

        private void UnloadSceneByName(string sceneName)
        { 
            if (SceneManager.GetSceneByName(sceneName).IsValid()) SceneManager.UnloadSceneAsync(sceneName);
        }

        #region UIScene

        public async UniTask LoadUIScene()
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(UI, LoadSceneMode.Single);
            asyncOperation.completed += HandleCompletion;
            await asyncOperation;
            return;

            void HandleCompletion(AsyncOperation operation)
            {
                operation.completed -= HandleCompletion;
            }
        }

        #endregion
        
        #region MainMenu

        public async UniTask LoadMainMenu()
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(MAIN_MENU, LoadSceneMode.Additive);
            asyncOperation.completed += HandleCompletion;
            await asyncOperation;
            return;

            void HandleCompletion(AsyncOperation operation)
            {
                operation.completed -= HandleCompletion;
            }
        }

        public async UniTask UnloadMainMenu()
        {
            AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(MAIN_MENU); 
            asyncOperation.completed += HandleCompletion;
            await asyncOperation;
            return;

            void HandleCompletion(AsyncOperation operation)
            {
                operation.completed -= HandleCompletion;
            }
        }
        
        #endregion

        #region Gameplay

        public async UniTask LoadGameplayScene()
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(GAMEPLAY, LoadSceneMode.Additive);
            asyncOperation.completed += HandleCompletion;
            await asyncOperation;
            return;

            void HandleCompletion(AsyncOperation operation)
            {
                operation.completed -= HandleCompletion;
            }
        }

        public async UniTask UnloadGameplayScene()
        {
            AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(GAMEPLAY); 
            asyncOperation.completed += HandleCompletion;
            await asyncOperation;
            return;

            void HandleCompletion(AsyncOperation operation)
            {
                operation.completed -= HandleCompletion;
            }
        }

        #endregion
        
   }
}