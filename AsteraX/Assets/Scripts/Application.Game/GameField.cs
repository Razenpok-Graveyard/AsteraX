using System.Threading;
using AsteraX.Application.Game.Asteroids;
using AsteraX.Domain.GameSession;
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
            _playerShip.transform.position = Vector3.zero;
            _playerShip.SetActive(true);
        }
    }
}