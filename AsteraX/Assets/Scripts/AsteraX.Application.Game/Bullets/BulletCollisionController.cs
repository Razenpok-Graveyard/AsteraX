using AsteraX.Application.Game.Asteroids;
using AsteraX.Infrastructure;
using Common.Application;
using Common.Functional;
using UnityEngine;
using VContainer;

namespace AsteraX.Application.Game.Bullets
{
    public class BulletCollisionController : MonoBehaviour
    {
        private IRequestHandler<Command, Result> _commandHandler;

        [Inject]
        public void Construct(IRequestHandler<Command, Result> commandHandler)
        {
            _commandHandler = commandHandler;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<AsteroidInstance>(out var asteroid))
            {
                Destroy(gameObject);
                var request = new Command {AsteroidId = asteroid.Id};
                _commandHandler.Handle(request);
            }
        }

        public class Command : IRequest<Result>
        {
            public long AsteroidId { get; set; }
        }

        public class CommandHandler : RequestHandler<Command, Result>
        {
            private readonly IGameSessionRepository _gameSessionRepository;
            private readonly IApplicationTaskPublisher _applicationTaskPublisher;

            public CommandHandler(
                IGameSessionRepository gameSessionRepository,
                IApplicationTaskPublisher applicationTaskPublisher)
            {
                _gameSessionRepository = gameSessionRepository;
                _applicationTaskPublisher = applicationTaskPublisher;
            }

            protected override Result Handle(Command command)
            {
                var gameSession = _gameSessionRepository.GetCurrentSession();
                var maybeAsteroid = gameSession.LevelAttempt.Asteroids
                    .TryFirst(a => a.Id == command.AsteroidId);
                if (maybeAsteroid.HasNoValue)
                {
                    return Result.Failure($"Cannot destroy dead asteroid {command.AsteroidId}");
                }

                gameSession.CollideAsteroidWithBullet(maybeAsteroid.Value);
                _gameSessionRepository.Save(gameSession);

                _applicationTaskPublisher.Publish(new DestroyAsteroid
                {
                    Id = command.AsteroidId
                });

                return Result.Success();
            }
        }
    }
}