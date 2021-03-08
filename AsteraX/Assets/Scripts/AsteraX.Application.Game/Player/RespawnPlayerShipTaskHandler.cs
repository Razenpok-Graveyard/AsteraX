using System.Threading;
using AsteraX.Application.Game.Levels;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AsteraX.Application.Game.Player
{
    public class RespawnPlayerShipTaskHandler : AsyncApplicationTaskHandler<RespawnPlayerShip>
    {
        [SerializeField] private LevelBounds _levelBounds;
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

        private Vector3 FindSafeSpawnPoint()
        {
            var padding = new Vector2(4, 4);
            return _levelBounds.FindSafePosition(padding);
        }
    }
}