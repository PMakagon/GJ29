using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Develop.StunGames.GameJam29.Runtime.Services
{
    public class SceneLoaderService
    {
        //public const string MAIN_MENU = "MainMenu";
        public const string UI_SCENE = "UI";
        public const string CORE_SCENE = "Core";
        
        public void LoadSceneByName(string sceneName)
        {
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }

        public void UnloadSceneByName(string sceneName)
        { 
            if (SceneManager.GetSceneByName(sceneName).IsValid()) SceneManager.UnloadSceneAsync(sceneName);
        }
   }
}