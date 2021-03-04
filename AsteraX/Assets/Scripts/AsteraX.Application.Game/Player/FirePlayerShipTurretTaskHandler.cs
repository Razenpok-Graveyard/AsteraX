using System;
using AsteraX.Infrastructure;
using Common.Application;
using UniRx;
using UnityEngine;
using VContainer;

namespace AsteraX.Application.Game.Player
{
    public class FirePlayerShipTurretTaskHandler : ApplicationTaskHandler<FirePlayerShipTurret>
    {
        [SerializeField] private Transform _turret;
        [SerializeField] private Transform _shootingPoint;

        private IApplicationTaskPublisher _applicationTaskPublisher;
        private IRequestHandler<Query, Model> _requestHandler;

        private bool _isPlayerAlive;

        [Inject]
        public void Construct(
            IApplicationTaskPublisher applicationTaskPublisher,
            IRequestHandler<Query, Model> requestHandler)
        {
            _applicationTaskPublisher = applicationTaskPublisher;
            _requestHandler = requestHandler;
        }

        private void Start()
        {
            var model = _requestHandler.Handle(new Query());
            model.IsPlayerAlive
                .Subscribe(value => _isPlayerAlive = value)
                .AddTo(this);
        }

        protected override void Handle(FirePlayerShipTurret message)
        {
            if (!_isPlayerAlive)
            {
                return;
            }

            var bulletPosition = _shootingPoint.position;
            var turretPosition = _turret.position;
            var direction = (bulletPosition - turretPosition).normalized;
            var spawnBulletMessage = new SpawnBullet
            {
                WorldPosition = turretPosition,
                Direction = direction
            };
            _applicationTaskPublisher.PublishTask(spawnBulletMessage);
        }

        public class Model
        {
            public IObservable<bool> IsPlayerAlive { get; set; }
        }

        public class Query : IRequest<Model>
        {
        }

        public class QueryHandler : RequestHandler<Query, Model>
        {
            private readonly IGameSessionObservableModelRepository _modelRepository;

            public QueryHandler(IGameSessionObservableModelRepository modelRepository)
            {
                _modelRepository = modelRepository;
            }

            protected override Model Handle(Query request)
            {
                var observableModel = _modelRepository.GetObservableModel();
                return new Model
                {
                    IsPlayerAlive = observableModel.IsPlayerAlive
                };
            }
        }
    }
}