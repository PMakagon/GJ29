using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Project.Develop.StunGames.GameJam29.Runtime.Gameplay
{
    public sealed class GameplayScope : LifetimeScope
    {
        [SerializeField] private GameConfig _gameConfig;
        [SerializeField] private LevelGenerator _levelGenerator;
        [SerializeField] private PlayerSpawner _playerSpawner;
        
        protected override void Configure(IContainerBuilder builder)
        {
            BindServices(builder);
            BindScene(builder);
        }

        private void BindServices(IContainerBuilder builder)
        {
            builder.RegisterComponent<GameConfig>(_gameConfig);
            builder.RegisterComponent<LevelGenerator>(_levelGenerator);
            builder.RegisterComponent<PlayerSpawner>(_playerSpawner);
            builder.Register<MatchController>(Lifetime.Singleton);
        }

        private void BindScene(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameplayFlow>();
        }
    }
}