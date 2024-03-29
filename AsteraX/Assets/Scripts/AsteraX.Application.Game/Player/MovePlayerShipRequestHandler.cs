﻿using AsteraX.Application.Game.Requests;
using Razensoft.Mediator;
using UnityEngine;

namespace AsteraX.Application.Game.Player
{
    public class MovePlayerShipRequestHandler : OutputRequestHandler<MovePlayerShip>
    {
        [SerializeField] private Transform _ship;
        [SerializeField] private PlayerShipSettings _settings;
        [SerializeField] private PlayerShipTrail _playerShipTrail;

        protected override void Handle(MovePlayerShip task)
        {
            var movement = task.Movement;
            var translationVector = new Vector2(movement.x, movement.y);
            var rigidBody = _ship.GetComponent<Rigidbody>();
            rigidBody.velocity = translationVector * _settings.MaximumSpeed;

            var rotationVector = new Vector2(movement.y, -movement.x);
            var rotation = rotationVector * _settings.MaximumTilt;
            _ship.localRotation = Quaternion.Euler(rotation.x, rotation.y, 0);
            
            _playerShipTrail.UpdateDirection(task.Movement);
        }
    }
}