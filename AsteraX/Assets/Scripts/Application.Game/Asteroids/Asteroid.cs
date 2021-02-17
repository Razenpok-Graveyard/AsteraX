using AsteraX.Application.Game.Commands;
using UniTaskPubSub;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AsteraX.Application.Game.Asteroids
{
    public class Asteroid : MonoBehaviour
    {
        private int _size;
        private int _childCount;
        private float _speed;
        private float _rotationSpeed;
        private Vector3 _rotationVector;

        public void Initialize(int size, int childCount, float speed, float rotationSpeed)
        {
            _size = size;
            _childCount = childCount;
            transform.localScale = Vector3.one * size;
            _speed = speed;
            _rotationSpeed = rotationSpeed;
            _rotationVector = Random.onUnitSphere;
        }

        private void Update()
        {
            var command = new TranslateGameFieldObjectCommand(
                gameObject, 
                Vector3.forward * Time.deltaTime * _speed);
            AsyncMessageBus.Default.Publish(command);
            transform.Rotate(_rotationVector, _rotationSpeed);
        }

        private void OnDestroy()
        {
            if (_size == 1)
            {
                return;
            }

            for (var i = 0; i < _childCount; i++)
            {
                var position = transform.position + Random.onUnitSphere / 5;
                position.z = 0;
                AsyncMessageBus.Default.Publish(new SpawnAsteroidCommand(_size - 1, position, Random.rotation));
            }
        }
    }
}