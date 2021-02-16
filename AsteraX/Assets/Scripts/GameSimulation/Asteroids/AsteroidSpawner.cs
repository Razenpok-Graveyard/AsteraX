using Commands;
using UnityEngine;
using Random = System.Random;

namespace Asteroids
{
    public class AsteroidSpawner : MonoBehaviour
    {
        private static readonly Random Random = new Random();

        [SerializeField] private AsteroidSettings _asteroidSettings;

        private bool isApplicationQuitting;

        private void Awake()
        {
            this.Subscribe<SpawnAsteroidCommand>(Handle);
            UnityEngine.Application.quitting += () => isApplicationQuitting = true;
        }

        private void Handle(SpawnAsteroidCommand command)
        {
            if (isApplicationQuitting)
            {
                return;
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
        }
    }
}