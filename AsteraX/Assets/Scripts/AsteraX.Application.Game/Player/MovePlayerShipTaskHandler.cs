using System;
using AsteraX.Application.Tasks.Game;
using AsteraX.Infrastructure;
using Common.Application;
using Common.Application.Unity;
using UniRx;
using UnityEngine;
using VContainer;

namespace AsteraX.Application.Game.Player
{
    public class MovePlayerShipTaskHandler : ApplicationTaskHandler<MovePlayerShip>
    {
        [SerializeField] private Transform _ship;
        [SerializeField] private PlayerShipSettings _settings;
        [SerializeField] private PlayerShipTrail _playerShipTrail;

        private IRequestHandler<Query, Model> _requestHandler;

        private bool _isPlayerAlive;

        [Inject]
        public void Construct(IRequestHandler<Query, Model> requestHandler)
        {
            _requestHandler = requestHandler;
        }

        private void Start()
        {
            var model = _requestHandler.Handle(new Query());
            model.IsPlayerAlive
                .Subscribe(value => _isPlayerAlive = value)
                .AddTo(this);
        }

        protected override void Handle(MovePlayerShip task)
        {
            if (!_isPlayerAlive)
            {
                return;
            }

            var movement = task.Movement;
            var translationVector = new Vector2(movement.x, movement.y);
            var rigidBody = _ship.GetComponent<Rigidbody>();
            rigidBody.velocity = translationVector * _settings.MaximumSpeed;

            var rotationVector = new Vector2(movement.y, -movement.x);
            var rotation = rotationVector * _settings.MaximumTilt;
            _ship.localRotation = Quaternion.Euler(rotation.x, rotation.y, 0);
            
            _playerShipTrail.UpdateDirection(task.Movement);
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
            private readonly IGameSessionObservableRepository _observableRepository;

            public QueryHandler(IGameSessionObservableRepository observableRepository)
            {
                _observableRepository = observableRepository;
            }

            protected override Model Handle(Query request)
            {
                var gameSession = _observableRepository.Get();
                return new Model
                {
                    IsPlayerAlive = gameSession.IsPlayerAlive
                };
            }
        }
    }
}