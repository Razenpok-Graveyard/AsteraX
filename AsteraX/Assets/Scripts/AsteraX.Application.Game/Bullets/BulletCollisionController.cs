using System.Collections.Generic;
using System.Linq;
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

namespace AsteraX.Application.Game.Bullets
{
    public class BulletCollisionController : MonoBehaviour
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
                Destroy(gameObject);
                var request = new Command {AsteroidId = asteroid.Id};
                _commandHandler.Handle(request).Forget();
            }
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
                gameSession.CollideAsteroidWithBullet(command.AsteroidId);
                _gameSessionRepository.Save();

                _taskPublisher.Publish(new DestroyAsteroid
                {
                    Id = command.AsteroidId
                });

                if (!gameSession.IsLevelCompleted)
                {
                    return;
                }

                var nextLevelId = gameSession.Level.Id + 1;
                var level = _levelRepository.GetLevel(nextLevelId);
                gameSession.StartLevel(level);
                _gameSessionRepository.Save();

                var asteroids = gameSession.GetAsteroids();
                var showLoadingScreen = ShowLoadingScreen.Create(level);
                var spawnAsteroids = SpawnAsteroids.Create(asteroids);

                await _taskPublisher.AsyncPublish(showLoadingScreen, ct);
                _taskPublisher.Publish(spawnAsteroids);
                await _taskPublisher.AsyncPublish(new HideLoadingScreen(), ct);
                _taskPublisher.Publish(new EnablePlayerInput());
                _taskPublisher.Publish(new UnpauseGame());
            }
        }
    }
}