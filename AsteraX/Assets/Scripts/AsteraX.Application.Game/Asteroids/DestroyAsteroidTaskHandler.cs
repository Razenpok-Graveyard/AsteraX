using System.Collections.Generic;
using System.Linq;
using AsteraX.Infrastructure;
using Common.Application;
using UnityEngine;
using VContainer;

namespace AsteraX.Application.Game.Asteroids
{
    public class DestroyAsteroidTaskHandler : ApplicationTaskHandler<DestroyAsteroid>
    {
        [SerializeField] private AsteroidInstanceContainer _instanceContainer;
        
        private IRequestHandler<Command, Model> _commandHandler;
        private IApplicationTaskPublisher _applicationTaskPublisher;

        [Inject]
        public void Construct(CommandHandler commandHandler, IApplicationTaskPublisher applicationTaskPublisher)
        {
            _commandHandler = commandHandler;
            _applicationTaskPublisher = applicationTaskPublisher;
        }

        protected override void Handle(DestroyAsteroid message)
        {
            var instance = _instanceContainer.Get(message.Id);
            var command = new Command
            {
                Id = message.Id
            };
            var model = _commandHandler.Handle(command);

            foreach (var child in model.Children)
            {
                var position = instance.gameObject.transform.position + Random.onUnitSphere / 5;
                position.z = 0;
                var spawnTask = new SpawnAsteroid
                {
                    Id = child.Id,
                    Direction = Random.rotation,
                    Size = child.Size,
                    WorldPosition = position
                };
                _applicationTaskPublisher.Publish(spawnTask);
            }

            _instanceContainer.Remove(message.Id);
            Destroy(instance.gameObject);
        }

        public class Model
        {
            public List<AsteroidDto> Children { get; set; }
        }

        public class AsteroidDto
        {
            public long Id { get; set; }
            public int Size { get; set; }

            public static AsteroidDto FromAsteroid(Domain.Game.Asteroid asteroid)
            {
                return new AsteroidDto
                {
                    Id = asteroid.Id,
                    Size = asteroid.Size
                };
            }
        }

        public class Command : IRequest<Model>
        {
            public long Id { get; set; }
        }

        public class CommandHandler : RequestHandler<Command, Model>
        {
            private readonly IGameSessionRepository _gameSessionRepository;

            public CommandHandler(IGameSessionRepository gameSessionRepository)
            {
                _gameSessionRepository = gameSessionRepository;
            }

            protected override Model Handle(Command command)
            {
                var gameSession = _gameSessionRepository.GetCurrentSession();
                var asteroid = gameSession.LevelAttempt.DestroyedAsteroids
                    .First(a => a.Id == command.Id);
                return new Model
                {
                    Children = asteroid.Children.Select(AsteroidDto.FromAsteroid).ToList()
                };
            }
        }
    }
}