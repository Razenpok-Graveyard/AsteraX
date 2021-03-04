using System.Threading;
using AsteraX.Application.Game.Asteroids;
using AsteraX.Infrastructure;
using Common.Application;
using Common.Functional;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace AsteraX.Application.Game.Player
{
    
    public class PlayerShipCollisionController : MonoBehaviour
    {
        private IAsyncRequestHandler<Command, Result> _commandHandler;

        [Inject]
        public void Construct(IAsyncRequestHandler<Command, Result> commandHandler)
        {
            _commandHandler = commandHandler;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<AsteroidInstance>(out var asteroid))
            {
                HandleCollision(asteroid.Id).Forget();
            }
        }

        private async UniTask HandleCollision(long asteroidId)
        {
            enabled = false;
            var command = new Command {AsteroidId = asteroidId};
            var (_, isFailure, error) = await _commandHandler.Handle(command);
            if (isFailure)
            {
                Debug.LogError(error);
            }
            enabled = true;
        }

        public class Command : IRequest<Result>
        {
            public long AsteroidId { get; set; }
        }

        public class CommandHandler : AsyncRequestHandler<Command, Result>
        {
            private readonly IGameSessionRepository _gameSessionRepository;
            private readonly IApplicationTaskPublisher _taskPublisher;

            public CommandHandler(
                IGameSessionRepository gameSessionRepository,
                IApplicationTaskPublisher taskPublisher)
            {
                _gameSessionRepository = gameSessionRepository;
                _taskPublisher = taskPublisher;
            }

            protected override async UniTask<Result> Handle(Command command, CancellationToken ct)
            {
                var gameSession = _gameSessionRepository.GetCurrentSession();
                var maybeAsteroid = gameSession.LevelAttempt.Asteroids
                    .TryFirst(a => a.Id == command.AsteroidId);
                if (maybeAsteroid.HasNoValue)
                {
                    return Result.Failure($"Cannot destroy dead asteroid {command.AsteroidId}");
                }

                gameSession.CollideAsteroidWithPlayerShip(maybeAsteroid.Value);
                _gameSessionRepository.Commit();

                var destroyAsteroidTask = new DestroyAsteroid
                {
                    Id = command.AsteroidId
                };
                _taskPublisher.PublishTask(destroyAsteroidTask);
                _taskPublisher.PublishTask(new DestroyPlayerShip());

                if (!gameSession.IsOver)
                {
                    const float respawnDelay = 2;
                    var respawnPlayerShipTask = new RespawnPlayerShip
                    {
                        Delay = respawnDelay
                    };
                    await _taskPublisher.PublishAsyncTask(respawnPlayerShipTask, ct);
                    gameSession.RespawnPlayer();
                    _gameSessionRepository.Commit();
                }

                return Result.Success();
            }
        }
    }
}