using System;
using System.Threading;
using AsteraX.GameSimulation.Commands;
using Cysharp.Threading.Tasks;
using MediatR;
using UnityEngine;
using Random = System.Random;

namespace AsteraX.GameSimulation.Player.Asteroids
{
    public class AsteroidSpawner : MonoBehaviour, IRequestHandler<SpawnAsteroidCommand>
    {
        private static readonly Random Random = new Random();

        [SerializeField] private AsteroidSettings _asteroidSettings;

        private bool isApplicationQuitting;

        private void Awake()
        {
            UnityEngine.Application.quitting += () => isApplicationQuitting = true;
        }

        public UniTask<Unit> Handle(SpawnAsteroidCommand command, CancellationToken cancellationToken)
        {
            if (isApplicationQuitting)
            {
                return Unit.Task;
            }

            const int childCount = 3;
            var prefabIndex = Random.Next(0, _asteroidSettings.Prefabs.Length);
            var prefab = _asteroidSettings.Prefabs[prefabIndex];
            var asteroid = Instantiate(prefab, command.WorldPosition, command.Direction);
            asteroid.Initialize(
                command.Size,
                childCount,
                _asteroidSettings.Speed,
                _asteroidSettings.RotationSpeed
            );
            return Unit.Task;
        }
    }
}