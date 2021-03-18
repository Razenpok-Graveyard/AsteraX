using AsteraX.Application.Game.Levels;
using AsteraX.Application.Game.Requests;
using Common.Application.Unity;
using UnityEngine;

namespace AsteraX.Application.Game.Asteroids
{
    public class SpawnAsteroidRequestHandler : OutputRequestHandler<SpawnAsteroids>
    {
        [SerializeField] private AsteroidInstanceContainer _instanceContainer;
        [SerializeField] private AsteroidSettings _asteroidSettings;
        [SerializeField] private LevelBounds _levelBounds;

        protected override void Handle(SpawnAsteroids message)
        {
            foreach (var asteroidDto in message.Asteroids)
            {
                var asteroid = InstantiateAsteroid(asteroidDto, GetRandomSpawnPosition(), transform);
                var physicsBody = asteroid.GetComponent<AsteroidPhysicsBody>();
                physicsBody.SetRandomVelocity();
            }
        }

        private GameObject InstantiateAsteroid(
            SpawnAsteroids.AsteroidDto asteroidDto,
            Vector3 position,
            Transform parent)
        {
            var prefabIndex = Random.Range(0, _asteroidSettings.Prefabs.Length);
            var prefab = _asteroidSettings.Prefabs[prefabIndex];
            var asteroid = Instantiate(prefab, position, Random.rotation, parent);
            asteroid.transform.localScale = Vector3.one * asteroidDto.Size / parent.lossyScale.x;

            var instance = AsteroidInstance.AddTo(asteroid, asteroidDto.Id);
            _instanceContainer.Add(asteroidDto.Id, instance);

            var childContainer = asteroid.AddComponent<AsteroidChildContainer>();

            foreach (var childDto in asteroidDto.Children)
            {
                var childPosition = asteroid.transform.position + Random.onUnitSphere;
                var child = InstantiateAsteroid(childDto, childPosition, asteroid.transform);
                var childPhysicsBody = child.GetComponent<AsteroidPhysicsBody>();
                childPhysicsBody.Disable();
                childContainer.Add(childPhysicsBody);
            }

            return asteroid;
        }

        private Vector3 GetRandomSpawnPosition()
        {
            var spawnPadding = new Vector2(0.5f, 0.5f);
            return _levelBounds.GetRandomPositionOutsideSafeArea(spawnPadding);
        }
    }
}