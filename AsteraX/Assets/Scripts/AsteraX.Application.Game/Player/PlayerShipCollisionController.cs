using System;
using System.Threading;
using AsteraX.Application.Game.Asteroids;
using AsteraX.Application.Game.Notifications;
using AsteraX.Application.Game.Requests;
using AsteraX.Application.UI.Requests;
using AsteraX.Domain.Game;
using AsteraX.Infrastructure;
using Razensoft.Mediator;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace AsteraX.Application.Game.Player
{
    public class PlayerShipCollisionController : MonoBehaviour
    {
        private IAsyncInputRequestHandler<Command> _commandHandler;

        [Inject]
        public void Construct(IAsyncInputRequestHandler<Command> commandHandler)
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

        public class CommandHandler : AsyncInputRequestHandler<Command>
        {
            private readonly ILevelRepository _levelRepository;
            private readonly GameSessionRepository _gameSessionRepository;
            private readonly IOutputMediator _mediator;

            public CommandHandler(
                ILevelRepository levelRepository,
                GameSessionRepository gameSessionRepository,
                IOutputMediator mediator)
            {
                _levelRepository = levelRepository;
                _gameSessionRepository = gameSessionRepository;
                _mediator = mediator;
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
                _mediator.Send(new DisablePlayerInput());
                _mediator.Send(new DestroyPlayerShip());
                _mediator.Send(destroyAsteroidTask);

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
                await _mediator.AsyncSend(showGameOverScreen, ct);
                _mediator.Send(new ClearAsteroids());
                _mediator.Send(respawnPlayerShip);
                await _mediator.AsyncSend(new HideGameOverScreen(), ct);
                _mediator.Send(new ShowMainMenuScreen());
                gameSession.Restart();
                _gameSessionRepository.Save();
            }

            private async UniTask StartNextLevel(GameSession gameSession, CancellationToken ct)
            {
                var nextLevelId = gameSession.Level.Id + 1;
                var level = _levelRepository.Get(nextLevelId);
                gameSession.StartLevel(level);
                _gameSessionRepository.Save();

                var levelReached = new LevelReached
                {
                    Id = level.Id
                };
                var asteroids = gameSession.GetAsteroids();
                var showLoadingScreen = ShowLoadingScreen.Create(level);
                var spawnAsteroids = SpawnAsteroids.Create(asteroids);

                _mediator.Publish(levelReached);
                await _mediator.AsyncSend(showLoadingScreen, ct);
                _mediator.Send(spawnAsteroids);

                gameSession.RespawnPlayer();
                _gameSessionRepository.Save();
                _mediator.Send(new RespawnPlayerShip());

                await _mediator.AsyncSend(new HideLoadingScreen(), ct);
                _mediator.Send(new UnpauseGame());
                _mediator.Send(new EnablePlayerInput());
            }

            private async UniTask Respawn(GameSession gameSession, CancellationToken ct)
            {
                var respawnPlayerShipTask = new RespawnPlayerShipWithVisuals
                {
                    Delay = TimeSpan.FromSeconds(2)
                };
                await _mediator.AsyncSend(respawnPlayerShipTask, ct);
                gameSession.RespawnPlayer();
                _gameSessionRepository.Save();
                _mediator.Send(new EnablePlayerInput());
            }
        }
    }
}