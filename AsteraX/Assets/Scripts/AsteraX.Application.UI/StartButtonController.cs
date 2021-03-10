using System.Threading;
using AsteraX.Application.Game;
using AsteraX.Infrastructure;
using Common.Application;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace AsteraX.Application.UI
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
                .Subscribe(_ => _commandHandler.Handle(new Command()))
                .AddTo(this);
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
                _gameSessionRepository.Save();

                _taskPublisher.PublishTask(new CloseMainMenu());
                await _taskPublisher.PublishAsyncTask(new LoadCurrentLevel(), ct);
                _taskPublisher.PublishTask(new ShowPauseButton());
            }
        }
    }
}