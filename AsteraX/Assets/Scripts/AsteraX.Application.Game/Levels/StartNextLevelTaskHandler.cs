using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AsteraX.Application.UI;
using AsteraX.Domain.Game;
using AsteraX.Infrastructure;
using Common.Application;
using Cysharp.Threading.Tasks;
using VContainer;
using static AsteraX.Application.Game.SpawnAsteroids;

namespace AsteraX.Application.Game.Levels
{
    public class StartNextLevelTaskHandler : AsyncApplicationTaskHandler<StartNextLevel>
    {
        private IAsyncRequestHandler<Command> _commandHandler;

        [Inject]
        public void Construct(IAsyncRequestHandler<Command> commandHandler)
        {
            _commandHandler = commandHandler;
        }

        protected override async UniTask Handle(StartNextLevel task, CancellationToken ct)
        {
            await _commandHandler.Handle(new Command(), ct);
        }

        public class Command : IRequest { }

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
                var level = _levelRepository.GetLevel();
                var gameSession = _gameSessionRepository.Get();
                gameSession.StartLevel(level);
                var asteroids = gameSession.GetAsteroids();

                var startTask = new ShowLoadingScreen
                {
                    Id = (int) level.Id,
                    Asteroids = level.AsteroidCount,
                    Children = level.AsteroidChildCount
                };
                await _taskPublisher.PublishAsyncTask(startTask, ct);
                
                var spawnTask = new SpawnAsteroids
                {
                    Asteroids = ToSpawnAsteroidsDto(asteroids)
                };
                _taskPublisher.PublishTask(spawnTask);

                await _taskPublisher.PublishAsyncTask(new HideLoadingScreen(), ct);
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