using System.Collections.Generic;
using System.Linq;
using AsteraX.Domain.Game;
using AsteraX.Infrastructure;
using Common.Application;
using VContainer;
using static AsteraX.Application.Game.SpawnAsteroids;

namespace AsteraX.Application.Game.Level
{
    public class StartNextLevelTaskHandler : ApplicationTaskHandler<StartNextLevel>
    {
        private IRequestHandler<Command> _commandHandler;

        [Inject]
        public void Construct(IRequestHandler<Command> commandHandler)
        {
            _commandHandler = commandHandler;
        }

        protected override void Handle(StartNextLevel task)
        {
            _commandHandler.Handle(new Command());
        }

        public class Command : IRequest { }

        public class CommandHandler : RequestHandler<Command>
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

            protected override void Handle(Command request)
            {
                var level = _levelRepository.GetLevel();
                var gameSession = _gameSessionRepository.Get();
                gameSession.StartLevel(level);
                var asteroids = gameSession.GetAsteroids();
                var spawnTask = new SpawnAsteroids
                {
                    Asteroids = ToSpawnAsteroidsDto(asteroids)
                };

                _taskPublisher.PublishTask(spawnTask);
            }

            private static List<AsteroidDto> ToSpawnAsteroidsDto(IEnumerable<Asteroid> asteroids)
            {
                return asteroids.Select(a => new AsteroidDto
                {
                    Id = a.Id,
                    Size = a.Size,
                    Children = ToSpawnAsteroidsDto(a.Children)
                }).ToList();
            }
        }
    }
}