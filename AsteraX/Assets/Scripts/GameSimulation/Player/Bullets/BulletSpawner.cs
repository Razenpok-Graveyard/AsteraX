using AsteraX.GameSimulation.Commands;
using AsteraX.Mediator.Assets.Scripts;
using UnityEngine;
using VContainer;

namespace AsteraX.GameSimulation.Player.Bullets
{
    public class BulletSpawner : MonoBehaviour, IRequestHandler<SpawnBulletCommand>
    {
        [SerializeField] private BulletSettings _bulletSettings;
        private ISender _sender;

        [Inject]
        public void Construct(ISender sender) => _sender = sender;

        public void Handle(SpawnBulletCommand command)
        {
            var bullet = Instantiate(_bulletSettings.Prefab, command.WorldPosition, command.Direction);
            bullet.Initialize(_bulletSettings.Speed, _bulletSettings.Lifetime, _sender);
        }
    }
}