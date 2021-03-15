using System;
using System.Threading;
using AsteraX.Application.Game.Levels;
using AsteraX.Application.Tasks.Game;
using Common.Application.Unity;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AsteraX.Application.Game.Player
{
    public class RespawnPlayerShipTaskHandler : MonoBehaviour
    {
        [SerializeField] private LevelBounds _levelBounds;
        [SerializeField] private GameObject _playerShip;
        [SerializeField] private ParticleSystem _exhaustTrail;
        [SerializeField] private ParticleSystem _appearEffect;

        private void Awake()
        {
            this.Subscribe<RespawnPlayerShipWithVisuals>(Handle);
            this.Subscribe<RespawnPlayerShip>(Handle);
        }

        private async UniTask Handle(RespawnPlayerShipWithVisuals task, CancellationToken ct)
        {
            var effectDelay = TimeSpan.FromMilliseconds(500);
            var initialDelay = task.Delay - effectDelay;
            if (initialDelay > TimeSpan.Zero)
            {
                await UniTask.Delay(initialDelay, cancellationToken: ct);
            }
            _playerShip.transform.position = FindSafeSpawnPoint();
            var position = _playerShip.transform.position;
            position.z = _appearEffect.transform.position.z;
            var effect = Instantiate(_appearEffect, position, Quaternion.identity);
            DestroyWhenDone(effect).Forget();
            var actualEffectDelay = task.Delay - initialDelay;
            await UniTask.Delay(actualEffectDelay, cancellationToken: ct);
            _playerShip.SetActive(true);
            var emission = _exhaustTrail.emission;
            emission.enabled = true;
        }

        private void Handle(RespawnPlayerShip task)
        {
            _playerShip.transform.position = task.IntoInitialPosition ? Vector3.zero : FindSafeSpawnPoint();
            _playerShip.SetActive(true);
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