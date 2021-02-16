﻿using System.Threading;
using AsteraX.Domain.GameSession;
using Cysharp.Threading.Tasks;
using UniTaskPubSub;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _playerShip;
    
    private void Awake()
    {
        AsyncMessageBus.Default.Subscribe<GameOverEvent>(Handle);
    }

    private async UniTask Handle(GameOverEvent @event, CancellationToken ct)
    {
        await UniTask.Delay(2000, cancellationToken: ct);
        _playerShip.transform.position = Vector3.zero;
        _playerShip.SetActive(true);
    }
}