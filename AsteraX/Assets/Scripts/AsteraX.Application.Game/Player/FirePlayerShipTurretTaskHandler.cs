using AsteraX.Application.Game.Notifications;
using AsteraX.Application.Game.Requests;
using AsteraX.Infrastructure;
using Common.Application;
using Common.Application.Unity;
using UnityEngine;
using VContainer;

namespace AsteraX.Application.Game.Player
{
    public class FirePlayerShipTurretTaskHandler : OutputRequestHandler<FirePlayerShipTurret>
    {
        [SerializeField] private Transform _turret;
        [SerializeField] private Transform _shootingPoint;

        private IInputRequestHandler<Command> _commandHandler;

        [Inject]
        public void Construct(IInputRequestHandler<Command> commandHandler)
        {
            _commandHandler = commandHandler;
        }

        protected override void Handle(FirePlayerShipTurret message)
        {
            var bulletPosition = _shootingPoint.position;
            var turretPosition = _turret.position;
            var direction = (bulletPosition - turretPosition).normalized;
            var command = new Command
            {
                WorldPosition = turretPosition,
                Direction = direction
            };
            _commandHandler.Handle(command);
        }

        public class Command : IRequest
        {
            public Vector3 WorldPosition { get; set; }
            public Vector3 Direction { get; set; }
        }

        public class CommandHandler : InputRequestHandler<Command>
        {
            private readonly GameSessionRepository _gameSessionRepository;
            private readonly IOutputMediator _mediator;

            public CommandHandler(
                GameSessionRepository gameSessionRepository,
                IOutputMediator mediator)
            {
                _gameSessionRepository = gameSessionRepository;
                _mediator = mediator;
            }

            protected override void Handle(Command command)
            {
                var gameSession = _gameSessionRepository.Get();
                if (!gameSession.CanShoot)
                {
                    return;
                }

                var spawnBulletMessage = new SpawnBullet
                {
                    WorldPosition = command.WorldPosition,
                    Direction = command.Direction
                };
                _mediator.Send(spawnBulletMessage);
                _mediator.Publish(new ShotFired());
            }
        }
    }
}