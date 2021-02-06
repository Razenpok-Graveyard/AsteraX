﻿using System.Threading;
using System.Threading.Tasks;
using AsteraX.GameSimulation.Commands;
using MediatR;
using UnityEngine;
using VContainer;

namespace AsteraX.GameSimulation.Player
{
    public class PlayerShipMovementPresenter : MonoBehaviour, IRequestHandler<MoveShipCommand>
    {
        [SerializeField] private Transform _ship;
        [SerializeField] private PlayerShipSettings _settings;

        private ISender _sender;

        [Inject]
        public void Construct(ISender sender) => _sender = sender;

        public Task<Unit> Handle(MoveShipCommand command, CancellationToken cancellationToken)
        {
            if (!isActiveAndEnabled)
            {
                return Unit.Task;
            }

            var movement = command.Movement;
            var translationVector = new Vector2(movement.x, movement.y);
            var translation = translationVector * _settings.MaximumSpeed * Time.deltaTime;
            _sender.Send(new TranslateGameFieldObjectCommand(gameObject, translation));

            var rotationVector = new Vector2(movement.y, -movement.x);
            var rotation = rotationVector * _settings.MaximumTilt;
            _ship.localRotation = Quaternion.Euler(rotation.x, rotation.y, 0);
            return Unit.Task;
        }
    }
}