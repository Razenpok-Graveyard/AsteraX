using AsteraX.Application.Game;
using AsteraX.Application.Game.Player;
using AsteraX.Infrastructure;
using AsteraX.Infrastructure.Data;
using JetBrains.Annotations;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace AsteraX.Startup
{
    public class GameSceneLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameField _gameField;
    
        protected override void Configure([NotNull] IContainerBuilder builder)
        {
            builder.RegisterInstance(_gameField).As<IGameField>();
            builder.Register<PlayerShipCollisionController.CommandHandler>(Lifetime.Transient);
            builder.Register<IGameSessionRepository, GameSessionRepository>(Lifetime.Singleton);
        }
    }
}