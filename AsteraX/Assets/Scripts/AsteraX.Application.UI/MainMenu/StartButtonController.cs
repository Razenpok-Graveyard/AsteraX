﻿using System.Threading;
using AsteraX.Application.Tasks.Game;
using AsteraX.Application.Tasks.UI;
using AsteraX.Infrastructure;
using Common.Application;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

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
                var level = _levelRepository.GetLevel(1);
                var gameSession = _gameSessionRepository.Get();
                gameSession.StartLevel(level);
                _gameSessionRepository.Save();

                var asteroids = gameSession.GetAsteroids();
                var showLoadingScreen = ShowLoadingScreen.Create(level);
                var spawnAsteroids = SpawnAsteroids.Create(asteroids);

                _taskPublisher.Publish(new HideMainMenuScreen());
                await _taskPublisher.AsyncPublish(showLoadingScreen, ct);
                _taskPublisher.Publish(spawnAsteroids);
                await _taskPublisher.AsyncPublish(new HideLoadingScreen(), ct);
                _taskPublisher.Publish(new ShowPauseButton());
                _taskPublisher.Publish(new UnpauseGame());
                _taskPublisher.Publish(new EnablePlayerInput());
            }
        }
    }
}