using AsteraX.Application;
using JetBrains.Annotations;
using Player;
using UniTaskPubSub;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameSceneLifetimeScope : LifetimeScope
{
    [SerializeField] private GameField _gameField;
    
    protected override void Configure([NotNull] IContainerBuilder builder)
    {
        builder.RegisterComponent(_gameField);
        builder.Register<PlayerShipCollisionController.CommandHandler>(Lifetime.Transient);
        builder.Register<IGameSessionRepository, GameSessionRepository>(Lifetime.Singleton);
        
        DomainEvents.RegisterGeneralHandler(e => AsyncMessageBus.Default.PublishAsync(e));
    }
}