using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AsteraX.Application.Tasks.Game;
using AsteraX.Application.Tasks.UI;
using AsteraX.Domain.Game;
using AsteraX.Infrastructure;
using Common.Application;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using static AsteraX.Application.Tasks.Game.SpawnAsteroids;

namespace AsteraX.Application.UI.MainMenu
{
    public class StartButtonController : MonoBehaviour
    {
        [SerializeField] private Button _button;

        private IAsyncRequestHandler<Command> _commandHandler;

        [Inject]
        public void Construct(IAsyncRequestHandler<Command> commandHandler)
        {
            _commandHandler = commandHandler;
        }

        private void Start()
        {
            gameObject.OnEnableAsObservable()
                .Subscribe(_ => _button.interactable = true)
                .AddTo(this);

            _button.OnClickAsObservable()
                .Subscribe(_ => _commandHandler.Handle(new Command()).Forget())
                .AddTo(this);
        }

        public class Command : IAsyncRequest { }

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
                _gameSessionRepository.Save();

                var asteroids = gameSession.GetAsteroids();
                
                var showLoadingScreen = new ShowLoadingScreen
                {
                    Id = (int) level.Id,
                    Asteroids = level.AsteroidCount,
                    Children = level.AsteroidChildCount
                };
                var spawnAsteroids = new SpawnAsteroids
                {
                    Asteroids = ToSpawnAsteroidsDto(asteroids)
                };

                _taskPublisher.PublishTask(new CloseMainMenu());
                await _taskPublisher.PublishAsyncTask(showLoadingScreen, ct);
                _taskPublisher.PublishTask(spawnAsteroids);
                await _taskPublisher.PublishAsyncTask(new HideLoadingScreen(), ct);
                _taskPublisher.PublishTask(new ShowPauseButton());
                _taskPublisher.PublishTask(new UnpauseGame());
                _taskPublisher.PublishTask(new EnablePlayerControls());
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