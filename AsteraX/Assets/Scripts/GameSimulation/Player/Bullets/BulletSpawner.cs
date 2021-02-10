using AsteraX.GameSimulation.Commands;
using MediatR.Unity;
using UnityEngine;

namespace AsteraX.GameSimulation.Player.Bullets
{
    public class BulletSpawner : MonoBehaviour
    {
        [SerializeField] private BulletSettings _bulletSettings;

        private bool isApplicationQuitting;

        private void Awake()
        {
            this.RegisterRequestHandler<SpawnBulletCommand>(Handle);
            UnityEngine.Application.quitting += () => isApplicationQuitting = true;
        }

        private void Handle(SpawnBulletCommand command)
        {
            var bullet = Instantiate(_bulletSettings.Prefab, command.WorldPosition, command.Direction);
            bullet.Initialize(_bulletSettings.Speed, _bulletSettings.Lifetime);
        }
    }
}