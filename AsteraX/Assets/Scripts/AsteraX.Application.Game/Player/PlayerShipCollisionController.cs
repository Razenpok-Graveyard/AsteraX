using System;
using AsteraX.Application.Game.Asteroids;
using AsteraX.Infrastructure;
using Common.Application;
using Common.Functional;
using UnityEngine;
using VContainer;

namespace AsteraX.Application.Game.Player
{
    
    public class PlayerShipCollisionController : MonoBehaviour
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
                var command = new Command {AsteroidId = asteroid.Id};
                var (_, isFailure, error) = _commandHandler.Handle(command);
                if (isFailure)
                {
                    Debug.LogError(error);
                }
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

                gameSession.CollideAsteroidWithPlayerShip(maybeAsteroid.Value);
                _gameSessionRepository.Save(gameSession);

                _applicationTaskPublisher.Publish(new DestroyAsteroid
                {
                    Id = command.AsteroidId
                });
                _applicationTaskPublisher.Publish(new DestroyPlayerShip());

                return Result.Success();
            }
        }
    }
}