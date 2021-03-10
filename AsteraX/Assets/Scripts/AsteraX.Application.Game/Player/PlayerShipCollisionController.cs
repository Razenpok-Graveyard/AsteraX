using System.Threading;
using AsteraX.Application.Game.Asteroids;
using AsteraX.Application.Tasks.Game;
using AsteraX.Application.Tasks.UI;
using AsteraX.Infrastructure;
using Common.Application;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace AsteraX.Application.Game.Player
{
    public class PlayerShipCollisionController : MonoBehaviour
    {
        private IAsyncRequestHandler<Command> _commandHandler;

        [Inject]
        public void Construct(IAsyncRequestHandler<Command> commandHandler)
        {
            _commandHandler = commandHandler;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<AsteroidInstance>(out var asteroid))
            {
                HandleCollisionAsync(asteroid.Id).Forget();
            }
        }

        private async UniTask HandleCollisionAsync(long asteroidId)
        {
            enabled = false;
            var command = new Command {AsteroidId = asteroidId};
            await _commandHandler.Handle(command);
            enabled = true;
        }

        public class Command : IAsyncRequest
        {
            public long AsteroidId { get; set; }
        }

        public class CommandHandler : AsyncRequestHandler<Command>
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

            protected override async UniTask Handle(Command command, CancellationToken ct)
            {
                var gameSession = _gameSessionRepository.Get();
                gameSession.CollideAsteroidWithPlayerShip(command.AsteroidId);
                _gameSessionRepository.Save();

                var destroyAsteroidTask = new DestroyAsteroid
                {
                    Id = command.AsteroidId
                };
                _taskPublisher.PublishTask(destroyAsteroidTask);
                _taskPublisher.PublishTask(new DestroyPlayerShip());

                if (gameSession.IsOver)
                {
                    var showGameOverScreen = new ShowGameOverScreen
                    {
                        Level = (int) gameSession.Level.Id,
                        Score = gameSession.Score
                    };
                    await _taskPublisher.PublishAsyncTask(showGameOverScreen, ct);
                }
                else
                {
                    const float respawnDelay = 2;
                    var respawnPlayerShipTask = new RespawnPlayerShip
                    {
                        Delay = respawnDelay
                    };
                    await _taskPublisher.PublishAsyncTask(respawnPlayerShipTask, ct);
                    gameSession.RespawnPlayer();
                    _gameSessionRepository.Save();
                }
            }
        }
    }
}