using System.Threading;
using AsteraX.GameSimulation.Commands;
using Cysharp.Threading.Tasks;
using MediatR;
using UnityEngine;
using static MediatR.MediatorSingleton;

namespace AsteraX.GameSimulation.Player
{
    public class PlayerShipMovementPresenter : MonoBehaviour, IRequestHandler<MoveShipCommand>
    {
        [SerializeField] private Transform _ship;
        [SerializeField] private PlayerShipSettings _settings;

        public UniTask<Unit> Handle(MoveShipCommand command, CancellationToken cancellationToken)
        {
            if (!isActiveAndEnabled)
            {
                return Unit.Task;
            }

            var movement = command.Movement;
            var translationVector = new Vector2(movement.x, movement.y);
            var translation = translationVector * _settings.MaximumSpeed * Time.deltaTime;
            Send(new TranslateGameFieldObjectCommand(gameObject, translation), cancellationToken);

            var rotationVector = new Vector2(movement.y, -movement.x);
            var rotation = rotationVector * _settings.MaximumTilt;
            _ship.localRotation = Quaternion.Euler(rotation.x, rotation.y, 0);
            return Unit.Task;
        }
    }
}