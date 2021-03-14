﻿using System;
using System.Threading;
using AsteraX.Application.Game.Asteroids;
using AsteraX.Application.Tasks.Game;
using AsteraX.Application.Tasks.UI;
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
                _taskPublisher.Publish(new DestroyPlayerShip());

                if (gameSession.IsOver)
                {
                    await ShowGameOver(gameSession, ct);
                    return;
                }

                if (gameSession.IsLevelCompleted)
                {
                    await ShowLevelCompleted(gameSession, ct);
                    return;
                }

                await Respawn(gameSession, ct);
            }

            private UniTask ShowGameOver(GameSession gameSession, CancellationToken ct)
            {
                var showGameOverScreen = new ShowGameOverScreen
                {
                    Level = (int) gameSession.Level.Id,
                    Score = gameSession.Score
                };
                return _taskPublisher.AsyncPublish(showGameOverScreen, ct);
            }

            private async UniTask ShowLevelCompleted(GameSession gameSession, CancellationToken ct)
            {
                var level = _levelRepository.GetLevel();
                gameSession.StartLevel(level);
                _gameSessionRepository.Save();

                var asteroids = gameSession.GetAsteroids();
                var showLoadingScreen = ShowLoadingScreen.Create(level);
                var spawnAsteroids = SpawnAsteroids.Create(asteroids);

                await _taskPublisher.AsyncPublish(showLoadingScreen, ct);
                _taskPublisher.Publish(spawnAsteroids);

                gameSession.RespawnPlayer();
                _gameSessionRepository.Save();
                await _taskPublisher.AsyncPublish(new RespawnPlayerShip(), ct);

                await _taskPublisher.AsyncPublish(new HideLoadingScreen(), ct);
                _taskPublisher.Publish(new UnpauseGame());
                _taskPublisher.Publish(new EnablePlayerInput());
            }

            private async UniTask Respawn(GameSession gameSession, CancellationToken ct)
            {
                var respawnPlayerShipTask = new RespawnPlayerShip
                {
                    Delay = TimeSpan.FromSeconds(2),
                    SpawnEffects = true
                };
                await _taskPublisher.AsyncPublish(respawnPlayerShipTask, ct);
                gameSession.RespawnPlayer();
                _gameSessionRepository.Save();
            }
        }
    }
}