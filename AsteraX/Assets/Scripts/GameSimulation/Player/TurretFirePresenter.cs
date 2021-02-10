using AsteraX.GameSimulation.Commands;
using MediatR.Unity;
using UnityEngine;

namespace AsteraX.GameSimulation.Player
{
    public class TurretFirePresenter : MonoBehaviour
    {
        [SerializeField] private Transform _turret;
        [SerializeField] private Transform _shootingPoint;

        private void Awake()
        {
            this.RegisterRequestHandler<FireCommand>(Handle);
        }

        private void Handle(FireCommand command)
        {
            var bulletPosition = _shootingPoint.position;
            var turretPosition = _turret.position;
            var direction = Quaternion.LookRotation(bulletPosition - turretPosition);
            UnityMediator.Send(new SpawnBulletCommand(turretPosition, direction));
        }
    }
}