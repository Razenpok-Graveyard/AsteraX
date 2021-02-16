using System;
using System.Threading;
using AsteraX.Domain.GameSession;
using Asteroids;
using Cysharp.Threading.Tasks;
using UniTaskPubSub;
using UnityEngine;

public class GameField : MonoBehaviour
{
    [SerializeField] private GameObject _playerShip;

    private void Awake()
    {
        AsyncMessageBus.Default.Subscribe<GameOverEvent>(e => Debug.Log("Game over"));
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