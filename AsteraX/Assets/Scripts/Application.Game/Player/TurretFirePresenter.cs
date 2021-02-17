using AsteraX.Application.Game.Commands;
using AsteraX.Application.SharedKernel;
using UniTaskPubSub;
using UnityEngine;

namespace AsteraX.Application.Game.Player
{
    public class TurretFirePresenter : MonoBehaviour
    {
        [SerializeField] private Transform _turret;
        [SerializeField] private Transform _shootingPoint;

        private void Awake()
        {
            this.Subscribe<FireCommand>(Handle);
        }

        private void Handle()
        {
            var bulletPosition = _shootingPoint.position;
            var turretPosition = _turret.position;
            var direction = Quaternion.LookRotation(bulletPosition - turretPosition);
            AsyncMessageBus.Default.Publish(new SpawnBulletCommand(turretPosition, direction));
        }
    }
}