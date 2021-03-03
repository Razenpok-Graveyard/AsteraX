using System.Linq;
using System.Threading;
using AsteraX.Application.Game.Asteroids;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AsteraX.Application.Game.Player
{
    public class DestroyPlayerShipMessageHandler : MonoBehaviour
    {
        [SerializeField] private GameObject _playerShip;

        private void Awake()
        {
            this.Subscribe<DestroyPlayerShip>(DestroyPlayerShip);
        }

        private void DestroyPlayerShip(DestroyPlayerShip message)
        {
            DestroyPlayerShipAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTask DestroyPlayerShipAsync(CancellationToken ct = default)
        {
            _playerShip.SetActive(false);
            await UniTask.Delay(2000, cancellationToken: ct);
            _playerShip.transform.position = FindSafeSpawnPoint();
            _playerShip.SetActive(true);
        }

        private Vector3 FindSafeSpawnPoint()
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

        private Vector3 GetRandomFieldPoint()
        {
            return new Vector3(
                Mathf.Lerp(-12, 12, Random.value),
                Mathf.Lerp(-5, 5, Random.value),
                0
            );
        }
    }
}