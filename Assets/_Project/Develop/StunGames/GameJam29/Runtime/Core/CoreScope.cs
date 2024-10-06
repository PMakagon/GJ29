using VContainer;
using VContainer.Unity;

namespace _Project.Develop.StunGames.EmptyProject.Runtime.Core
{
    public sealed class CoreScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            BindScene(builder);
        }
        
        private void BindScene(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<CoreFlow>();
        }
    }
}