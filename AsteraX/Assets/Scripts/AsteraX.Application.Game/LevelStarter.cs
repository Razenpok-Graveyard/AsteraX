using System.Collections.Generic;
using System.Linq;
using AsteraX.Application.Common;
using AsteraX.Domain.Game;
using AsteraX.Infrastructure;
using UnityEngine;
using VContainer;

namespace AsteraX.Application.Game
{
    public class LevelStarter : MonoBehaviour
    {
        private IRequestHandler<Command, Model> _commandHandler;
        private IApplicationTaskPublisher _applicationTaskPublisher;

        [Inject]
        public void Construct(CommandHandler commandHandler, IApplicationTaskPublisher applicationTaskPublisher)
        {
            _commandHandler = commandHandler;
            _applicationTaskPublisher = applicationTaskPublisher;
        }

        private void Start()
        {
            var model = _commandHandler.Handle(new Command());
            foreach (var asteroid in model.Asteroids)
            {
                var position = GetRandomSpawnPosition();
                var direction = Random.rotation;
                var spawnTask = new SpawnAsteroid
                {
                    Id = asteroid.Id,
                    Size = asteroid.Size,
                    WorldPosition = position,
                    Direction = direction
                };
                _applicationTaskPublisher.Publish(spawnTask);
            }
        }

        private static Vector3 GetRandomSpawnPosition()
        {
            var minPosition = new Vector2(-15.5f, -8.5f);
            var maxPosition = new Vector2(15.5f, 8.5f);

            Vector2 randomPosition;
            do
            {
                randomPosition = new Vector2(
                    Random.Range(minPosition.x, maxPosition.x),
                    Random.Range(minPosition.y, maxPosition.y)
                );
            } while (IsInsideSafeArea(randomPosition));

            return randomPosition;
        }

        private static bool IsInsideSafeArea(Vector2 position)
        {
            var safeArea = Rect.MinMaxRect(-5, -4, 5, 4);
            return safeArea.Contains(position);
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
                var gameSession = _gameSessionRepository.GetCurrentSession();
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