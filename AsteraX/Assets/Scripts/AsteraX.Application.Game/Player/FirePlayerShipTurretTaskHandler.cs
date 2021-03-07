using AsteraX.Infrastructure;
using Common.Application;
using UnityEngine;
using VContainer;

namespace AsteraX.Application.Game.Player
{
    public class FirePlayerShipTurretTaskHandler : ApplicationTaskHandler<FirePlayerShipTurret>
    {
        [SerializeField] private Transform _turret;
        [SerializeField] private Transform _shootingPoint;

        private IRequestHandler<Command> _requestHandler;

        [Inject]
        public void Construct(IRequestHandler<Command> requestHandler)
        {
            _requestHandler = requestHandler;
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
            _requestHandler.Handle(command);
        }

        public class Command : IRequest
        {
            public Vector3 WorldPosition { get; set; }
            public Vector3 Direction { get; set; }
        }

        public class QueryHandler : RequestHandler<Command>
        {
            private readonly IGameSessionRepository _gameSessionRepository;
            private readonly IApplicationTaskPublisher _taskPublisher;

            public QueryHandler(
                IGameSessionRepository gameSessionRepository,
                IApplicationTaskPublisher taskPublisher)
            {
                _gameSessionRepository = gameSessionRepository;
                _taskPublisher = taskPublisher;
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
                _taskPublisher.PublishTask(spawnBulletMessage);
            }
        }
    }
}