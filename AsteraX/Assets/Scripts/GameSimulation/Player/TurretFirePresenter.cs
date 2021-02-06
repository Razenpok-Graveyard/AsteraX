using System.Threading;
using System.Threading.Tasks;
using AsteraX.GameSimulation.Commands;
using MediatR;
using UnityEngine;
using VContainer;

namespace AsteraX.GameSimulation.Player
{
    public class TurretFirePresenter : MonoBehaviour, IRequestHandler<FireCommand>
    {
        [SerializeField] private Transform _turret;
        [SerializeField] private Transform _shootingPoint;

        private ISender _sender;

        [Inject]
        public void Construct(ISender sender) => _sender = sender;

        public Task<Unit> Handle(FireCommand command, CancellationToken cancellationToken)
        {
            var bulletPosition = _shootingPoint.position;
            var turretPosition = _turret.position;
            var direction = Quaternion.LookRotation(bulletPosition - turretPosition);
            _sender.Send(new SpawnBulletCommand(turretPosition, direction));
            return Unit.Task;
        }
    }
}