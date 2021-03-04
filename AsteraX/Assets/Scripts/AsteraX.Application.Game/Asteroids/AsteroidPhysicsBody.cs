using UnityEngine;
using Random = UnityEngine.Random;

namespace AsteraX.Application.Game.Asteroids
{
    public class AsteroidPhysicsBody : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private MeshCollider _meshCollider;
        [SerializeField] private AsteroidSettings _asteroidSettings;

        public void Enable()
        {
            _rigidbody.isKinematic = false;
            _meshCollider.enabled = true;
        }

        public void Disable()
        {
            _rigidbody.isKinematic = true;
            _meshCollider.enabled = false;
        }

        public void SetRandomVelocity()
        {
            SetVelocity(Random.insideUnitCircle.normalized);
        }

        public void SetVelocity(Vector2 direction)
        {
            var velocity = Random.Range(
                _asteroidSettings.MinSpeed,
                _asteroidSettings.MaxSpeed
            );
            var angularVelocity = Random.Range(
                _asteroidSettings.MinRotationSpeed,
                _asteroidSettings.MaxRotationSpeed
            );
            _rigidbody.velocity = direction * velocity;
            _rigidbody.angularVelocity = Random.onUnitSphere * angularVelocity;
        }
    }
}