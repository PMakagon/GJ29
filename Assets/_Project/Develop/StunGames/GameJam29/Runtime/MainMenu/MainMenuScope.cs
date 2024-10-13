using VContainer;
using VContainer.Unity;

namespace _Project.Develop.StunGames.GameJam29.Runtime.MainMenu
{
    public class MainMenuScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            BindScene(builder);
        }
        
        private void BindScene(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<MainMenuFlow>();
        }
    }
}