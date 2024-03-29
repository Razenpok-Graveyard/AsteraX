﻿using System.Threading;
using AsteraX.Application.Game.Requests;
using AsteraX.Application.UI.Requests;
using AsteraX.Infrastructure;
using Razensoft.Mediator;
using Cysharp.Threading.Tasks;
using Razensoft.Mapper;
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

        private IAsyncInputRequestHandler<Command> _commandHandler;

        [Inject]
        public void Construct(IAsyncInputRequestHandler<Command> commandHandler)
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

        public class CommandHandler : AsyncInputRequestHandler<Command>
        {
            private readonly ILevelRepository _levelRepository;
            private readonly GameSessionRepository _gameSessionRepository;
            private readonly IOutputMediator _mediator;

            public CommandHandler(
                ILevelRepository levelRepository,
                GameSessionRepository gameSessionRepository,
                IOutputMediator mediator)
            {
                _levelRepository = levelRepository;
                _gameSessionRepository = gameSessionRepository;
                _mediator = mediator;
            }
            
            protected override async UniTask Handle(Command command, CancellationToken ct)
            {
                var level = _levelRepository.Get(1);
                var gameSession = _gameSessionRepository.Get();
                gameSession.StartLevel(level);
                _gameSessionRepository.Save();

                var asteroids = gameSession.GetAsteroids();
                var showLoadingScreen = ShowLoadingScreen.From(level);
                var spawnAsteroids = SpawnAsteroids.From(asteroids);

                _mediator.Send(new HideMainMenuScreen());
                await _mediator.AsyncSend(showLoadingScreen, ct);
                _mediator.Send(spawnAsteroids);
                await _mediator.AsyncSend(new HideLoadingScreen(), ct);
                //_mediator.Send(new ShowPauseButton());
                _mediator.Send(new UnpauseGame());
                _mediator.Send(new EnablePlayerInput());
            }
        }
    }
}