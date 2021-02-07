using System.Threading;
using AsteraX.GameSimulation.Commands;
using Cysharp.Threading.Tasks;
using MediatR;
using UnityEngine;
using static MediatR.MediatorSingleton;

namespace AsteraX.GameSimulation.Player
{
    public class TurretFirePresenter : MonoBehaviour, IRequestHandler<FireCommand>
    {
        [SerializeField] private Transform _turret;
        [SerializeField] private Transform _shootingPoint;

        public UniTask<Unit> Handle(FireCommand command, CancellationToken cancellationToken)
        {
            var bulletPosition = _shootingPoint.position;
            var turretPosition = _turret.position;
            var direction = Quaternion.LookRotation(bulletPosition - turretPosition);
            Send(new SpawnBulletCommand(turretPosition, direction), cancellationToken);
            return Unit.Task;
        }
    }
}