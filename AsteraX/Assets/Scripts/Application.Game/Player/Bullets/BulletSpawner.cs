using AsteraX.Application.Game.Commands;
using AsteraX.Application.SharedKernel;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace AsteraX.Application.Game.Player.Bullets
{
    public class BulletSpawner : MonoBehaviour
    {
        [SerializeField] private BulletSettings _bulletSettings;

        private IObjectResolver _objectResolver;

        [Inject]
        public void Construct(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }

        private void Awake()
        {
            this.Subscribe<SpawnBulletCommand>(Handle);
        }

        private void Handle(SpawnBulletCommand command)
        {
            var bullet = _objectResolver.Instantiate(_bulletSettings.Prefab, command.WorldPosition, command.Direction);
            bullet.Initialize(_bulletSettings.Speed, _bulletSettings.Lifetime);
        }
    }
}