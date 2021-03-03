using Common.Application;
using UnityEngine;
using VContainer;

namespace AsteraX.Application.Game.Player
{
    public class FirePlayerShipTurretTaskHandler : ApplicationTaskHandler<FirePlayerShipTurret>
    {
        [SerializeField] private Transform _turret;
        [SerializeField] private Transform _shootingPoint;

        private IApplicationTaskPublisher _applicationTaskPublisher;

        [Inject]
        public void Construct(IApplicationTaskPublisher applicationTaskPublisher)
        {
            _applicationTaskPublisher = applicationTaskPublisher;
        }

        protected override void Handle(FirePlayerShipTurret message)
        {
            var bulletPosition = _shootingPoint.position;
            var turretPosition = _turret.position;
            var direction = (bulletPosition - turretPosition).normalized;
            var spawnBulletMessage = new SpawnBullet
            {
                WorldPosition = turretPosition,
                Direction = direction
            };
            _applicationTaskPublisher.Publish(spawnBulletMessage);
        }
    }
}