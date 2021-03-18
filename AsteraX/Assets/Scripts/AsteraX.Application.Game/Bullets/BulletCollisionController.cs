using System.Threading;
using AsteraX.Application.Game.Asteroids;
using AsteraX.Application.Game.Notifications;
using AsteraX.Application.Game.Requests;
using AsteraX.Application.UI.Requests;
using AsteraX.Infrastructure;
using Common.Application;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace AsteraX.Application.Game.Bullets
{
    public class BulletCollisionController : MonoBehaviour
    {
        private IAsyncInputRequestHandler<Command> _commandHandler;

        [Inject]
        public void Construct(IAsyncInputRequestHandler<Command> commandHandler)
        {
            _commandHandler = commandHandler;
        }
        
        public bool IsLuckyShot { get; set; }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<AsteroidInstance>(out var asteroid))
            {
                Destroy(gameObject);
                var request = new Command
                {
                    AsteroidId = asteroid.Id,
                    IsLuckyShot = IsLuckyShot
                };
                _commandHandler.Handle(request).Forget();
            }
        }

        public class Command : IAsyncRequest
        {
            public long AsteroidId { get; set; }
            public bool IsLuckyShot { get; set; }
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
                gameSession.CollideAsteroidWithBullet(command.AsteroidId);
                _gameSessionRepository.Save();

                var destroyAsteroid = new DestroyAsteroid
                {
                    Id = command.AsteroidId
                };
                var highScoreUpdated = new HighScoreUpdated
                {
                    Score = gameSession.Score
                };
                _mediator.Send(destroyAsteroid);
                _mediator.Publish(new AsteroidShot
                {
                    IsLuckyShot = command.IsLuckyShot
                });
                _mediator.Publish(highScoreUpdated);

                if (!gameSession.IsLevelCompleted)
                {
                    return;
                }

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
                _mediator.Send(new DisablePlayerInput());
                await _mediator.AsyncSend(showLoadingScreen, ct);
                _mediator.Send(spawnAsteroids);
                await _mediator.AsyncSend(new HideLoadingScreen(), ct);
                _mediator.Send(new EnablePlayerInput());
                _mediator.Send(new UnpauseGame());
            }
        }
    }
}