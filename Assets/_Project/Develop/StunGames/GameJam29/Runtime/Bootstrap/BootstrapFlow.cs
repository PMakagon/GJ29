using UnityEngine;
using VContainer.Unity;

namespace _Project.Develop.StunGames.EmptyProject.Runtime.Bootstrap
{
    public class BootstrapFlow : IStartable
    {
        public BootstrapFlow()
        {
            
        }
        
        public void Start()
        {
            Debug.Log("BOOTSTRAP START");
        }
    }
}
