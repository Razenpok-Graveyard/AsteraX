using System.Collections.Generic;
using System.Linq;
using AsteraX.Domain.Game;
using AsteraX.Infrastructure;
using Common.Application;
using UnityEngine;
using VContainer;

namespace AsteraX.Application.Game.Level
{
    public class LevelStarter : MonoBehaviour
    {
        private IRequestHandler<Command, Model> _commandHandler;
        private IApplicationTaskPublisher _applicationTaskPublisher;

        [Inject]
        public void Construct(
            IRequestHandler<Command, Model> commandHandler,
            IApplicationTaskPublisher applicationTaskPublisher)
        {
            _commandHandler = commandHandler;
            _applicationTaskPublisher = applicationTaskPublisher;
        }

        private void Start()
        {
            var model = _commandHandler.Handle(new Command());
            var spawnTask = new SpawnAsteroids
            {
                Asteroids = model.Asteroids.Select(AsteroidDto.ToSpawnAsteroidsDto).ToList()
            };
            _applicationTaskPublisher.PublishTask(spawnTask);
        }

        public class Model
        {
            public List<AsteroidDto> Asteroids { get; set; } = new List<AsteroidDto>();
        }

        public class AsteroidDto
        {
            public long Id { get; set; }
            public int Size { get; set; }
            public List<AsteroidDto> Children { get; set; }

            public static AsteroidDto FromAsteroid(Asteroid asteroid)
            {
                return new AsteroidDto
                {
                    Id = asteroid.Id,
                    Size = asteroid.Size,
                    Children = asteroid.Children.Select(FromAsteroid).ToList()
                };
            }

            public static SpawnAsteroids.AsteroidDto ToSpawnAsteroidsDto(AsteroidDto asteroidDto)
            {
                return new SpawnAsteroids.AsteroidDto
                {
                    Id = asteroidDto.Id,
                    Size = asteroidDto.Size,
                    Children = asteroidDto.Children.Select(ToSpawnAsteroidsDto).ToList()
                };
            }
        }

        public class Command : IRequest<Model>
        {
        }

        public class CommandHandler : RequestHandler<Command, Model>
        {
            private readonly ILevelRepository _levelRepository;
            private readonly IGameSessionRepository _gameSessionRepository;

            public CommandHandler(
                ILevelRepository levelRepository,
                IGameSessionRepository gameSessionRepository)
            {
                _levelRepository = levelRepository;
                _gameSessionRepository = gameSessionRepository;
            }

            protected override Model Handle(Command request)
            {
                var level = _levelRepository.GetLevel();
                var gameSession = _gameSessionRepository.Get();
                gameSession.StartLevel(level);

                var levelAttempt = gameSession.LevelAttempt;
                return new Model
                {
                    Asteroids = levelAttempt.Asteroids.Select(AsteroidDto.FromAsteroid).ToList()
                };
            }
        }
    }
}