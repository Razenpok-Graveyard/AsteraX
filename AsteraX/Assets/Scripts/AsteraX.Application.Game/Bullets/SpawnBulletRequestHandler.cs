using System;
using AsteraX.Application.Game.Requests;
using Razensoft.Mediator;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace AsteraX.Application.Game.Bullets
{
    public class SpawnBulletRequestHandler : OutputRequestHandler<SpawnBullet>
    {
        [SerializeField] private BulletSettings _bulletSettings;

        private IObjectResolver _objectResolver;

        [Inject]
        public void Construct(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }

        protected override void Handle(SpawnBullet command)
        {
            var bullet = _objectResolver.Instantiate(
                _bulletSettings.Prefab,
                command.WorldPosition,
                Quaternion.LookRotation(command.Direction)
            );
            var rigidBody = bullet.GetComponent<Rigidbody>();
            rigidBody.velocity = command.Direction * _bulletSettings.Speed;
            DestroyAfterTime(bullet.gameObject, _bulletSettings.Lifetime).Forget();
        }

        private static async UniTaskVoid DestroyAfterTime(Object gameObject, float time)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(time));
            Destroy(gameObject);
        }
    }
}