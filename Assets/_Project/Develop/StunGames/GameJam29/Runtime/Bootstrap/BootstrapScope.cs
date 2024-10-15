using _Project.Develop.StunGames.GameJam29.Runtime.Services;
using VContainer;
using VContainer.Unity;

namespace _Project.Develop.StunGames.GameJam29.Runtime.Bootstrap
{
    public sealed class BootstrapScope : LifetimeScope
    {
        protected override void Awake()
        {
            IsRoot = true;
            DontDestroyOnLoad(this);
            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            BindServices(builder);
            BindScene(builder);
        }
        
        private void BindServices(IContainerBuilder builder)
        {
            builder.Register<SceneLoadingService>(Lifetime.Singleton);
            builder.Register<UIService>(Lifetime.Singleton);
        }
        
        private void BindScene(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<BootstrapFlow>();
        }
    }
}