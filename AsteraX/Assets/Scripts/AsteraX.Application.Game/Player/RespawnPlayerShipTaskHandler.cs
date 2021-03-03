using System.Linq;
using System.Threading;
using AsteraX.Application.Game.Asteroids;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AsteraX.Application.Game.Player
{
    public class RespawnPlayerShipTaskHandler : AsyncApplicationTaskHandler<RespawnPlayerShip>
    {
        [SerializeField] private GameObject _playerShip;
        [SerializeField] private ParticleSystem _exhaustTrail;
        [SerializeField] private ParticleSystem _appearEffect;

        protected override async UniTask Handle(RespawnPlayerShip task, CancellationToken ct)
        {
            await UniTask.Delay(1500, cancellationToken: ct);
            _playerShip.transform.position = FindSafeSpawnPoint();
            var position = _playerShip.transform.position;
            position.z = _appearEffect.transform.position.z;
            var effect = Instantiate(_appearEffect, position, Quaternion.identity);
            DestroyWhenDone(effect).Forget();
            await UniTask.Delay(500, cancellationToken: ct);
            _playerShip.SetActive(true);
            var emission = _exhaustTrail.emission;
            emission.enabled = true;
        }

        private static async UniTask DestroyWhenDone(ParticleSystem particleSystem)
        {
            await UniTask.WaitWhile(particleSystem.IsAlive);
            Destroy(particleSystem);
        }

        private static Vector3 FindSafeSpawnPoint()
        {
            var asteroids = FindObjectsOfType<AsteroidInstance>();
            for (int i = 0; i < 100; i++)
            {
                var point = GetRandomFieldPoint();
                var distances = asteroids.Select(a => Vector3.Distance(a.transform.position, point));
                if (distances.All(d => d > 8))
                {
                    return point;
                }
            }
            
            return GetRandomFieldPoint();
        }

        private static Vector3 GetRandomFieldPoint()
        {
            return new Vector3(
                Mathf.Lerp(-12, 12, Random.value),
                Mathf.Lerp(-5, 5, Random.value),
                0
            );
        }
    }
}