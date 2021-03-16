using System;
using System.Threading;
using AsteraX.Application.Game.Asteroids;
using AsteraX.Application.Game.Tasks;
using AsteraX.Application.UI.Tasks;
using AsteraX.Domain.Game;
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
            var command = new Command { AsteroidId = asteroidId };
            await _commandHandler.Handle(command);
            enabled = true;
        }

        public class Command : IAsyncRequest
        {
            public long AsteroidId { get; set; }
        }

        public class CommandHandler : AsyncRequestHandler<Command>
        {
            private readonly ILevelRepository _levelRepository;
            private readonly IGameSessionRepository _gameSessionRepository;
            private readonly IApplicationTaskPublisher _taskPublisher;

            public CommandHandler(
                ILevelRepository levelRepository,
                IGameSessionRepository gameSessionRepository,
                IApplicationTaskPublisher taskPublisher)
            {
                _levelRepository = levelRepository;
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
                _taskPublisher.Publish(destroyAsteroidTask);
                _taskPublisher.Publish(new DisablePlayerInput());
                _taskPublisher.Publish(new DestroyPlayerShip());

                if (gameSession.IsOver)
                {
                    await ShowGameOver(gameSession, ct);
                    return;
                }

                if (gameSession.IsLevelCompleted)
                {
                    await StartNextLevel(gameSession, ct);
                    return;
                }

                await Respawn(gameSession, ct);
            }

            private async UniTask ShowGameOver(GameSession gameSession, CancellationToken ct)
            {
                var showGameOverScreen = ShowGameOverScreen.Create(gameSession);
                var respawnPlayerShip = new RespawnPlayerShip{ IntoInitialPosition = true };
                await _taskPublisher.AsyncPublish(showGameOverScreen, ct);
                _taskPublisher.Publish(new ClearAsteroids());
                _taskPublisher.Publish(respawnPlayerShip);
                await _taskPublisher.AsyncPublish(new HideGameOverScreen(), ct);
                _taskPublisher.Publish(new ShowMainMenuScreen());
                gameSession.Restart();
                _gameSessionRepository.Save();
            }

            private async UniTask StartNextLevel(GameSession gameSession, CancellationToken ct)
            {
                var nextLevelId = gameSession.Level.Id + 1;
                var level = _levelRepository.GetLevel(nextLevelId);
                gameSession.StartLevel(level);
                _gameSessionRepository.Save();

                var asteroids = gameSession.GetAsteroids();
                var showLoadingScreen = ShowLoadingScreen.Create(level);
                var spawnAsteroids = SpawnAsteroids.Create(asteroids);

                await _taskPublisher.AsyncPublish(showLoadingScreen, ct);
                _taskPublisher.Publish(spawnAsteroids);

                gameSession.RespawnPlayer();
                _gameSessionRepository.Save();
                _taskPublisher.Publish(new RespawnPlayerShip());

                await _taskPublisher.AsyncPublish(new HideLoadingScreen(), ct);
                _taskPublisher.Publish(new UnpauseGame());
                _taskPublisher.Publish(new EnablePlayerInput());
            }

            private async UniTask Respawn(GameSession gameSession, CancellationToken ct)
            {
                var respawnPlayerShipTask = new RespawnPlayerShipWithVisuals
                {
                    Delay = TimeSpan.FromSeconds(2)
                };
                await _taskPublisher.AsyncPublish(respawnPlayerShipTask, ct);
                gameSession.RespawnPlayer();
                _gameSessionRepository.Save();
                _taskPublisher.Publish(new EnablePlayerInput());
            }
        }
    }
}