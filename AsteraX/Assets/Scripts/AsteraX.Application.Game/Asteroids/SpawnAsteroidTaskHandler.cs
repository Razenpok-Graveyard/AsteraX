using UnityEngine;

namespace AsteraX.Application.Game.Asteroids
{
    public class SpawnAsteroidTaskHandler : ApplicationTaskHandler<SpawnAsteroid>
    {
        [SerializeField] private AsteroidInstanceContainer _instanceContainer;
        [SerializeField] private AsteroidSettings _asteroidSettings;

        protected override void Handle(SpawnAsteroid message)
        {
            var asteroid = InstantiateAsteroid(message);
            var instance = AsteroidInstance.AddTo(asteroid, message.Id);
            _instanceContainer.Add(message.Id, instance);
        }

        private GameObject InstantiateAsteroid(SpawnAsteroid message)
        {
            var prefabIndex = Random.Range(0, _asteroidSettings.Prefabs.Length);
            var prefab = _asteroidSettings.Prefabs[prefabIndex];
            var asteroid = Instantiate(
                prefab,
                message.WorldPosition,
                message.Direction,
                transform
            );
            
            asteroid.transform.localScale = Vector3.one * message.Size;
            var movementDirection = Random.insideUnitCircle.normalized;
            var rigidBody = asteroid.GetComponent<Rigidbody>();
            rigidBody.velocity = movementDirection * _asteroidSettings.Speed;
            rigidBody.angularVelocity = Random.onUnitSphere * _asteroidSettings.RotationSpeed;
            return asteroid;
        }
    }
}