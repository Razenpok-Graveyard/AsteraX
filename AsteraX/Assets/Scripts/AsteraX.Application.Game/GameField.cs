using System.Linq;
using System.Threading;
using AsteraX.Application.Game.Asteroids;
using AsteraX.Domain;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AsteraX.Application.Game
{
    public class GameField : MonoBehaviour, IGameField
    {
        [SerializeField] private GameObject _playerShip;

        private void Awake()
        {
            this.SubscribeToDomain<GameOverEvent>(e => Debug.Log("Game over"));
        }

        public void Destroy(Asteroid asteroid)
        {
            Destroy(asteroid.gameObject);
        }

        public void DestroyPlayerShip()
        {
            DestroyPlayerShipAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTaskVoid DestroyPlayerShipAsync(CancellationToken ct = default)
        {
            _playerShip.SetActive(false);
            await UniTask.Delay(2000, cancellationToken: ct);
            _playerShip.transform.position = FindSafeSpawnPoint();
            _playerShip.SetActive(true);
        }

        private Vector3 FindSafeSpawnPoint()
        {
            var asteroids = FindObjectsOfType<Asteroid>();
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