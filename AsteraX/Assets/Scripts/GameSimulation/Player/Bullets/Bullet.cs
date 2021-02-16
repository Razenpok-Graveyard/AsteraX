using Asteroids;
using Commands;
using UniTaskPubSub;
using UnityEngine;

namespace Player.Bullets
{
    public class Bullet : MonoBehaviour
    {
        private float _speed;
        private float _lifetime;
        private float _passedLifetime;

        public void Initialize(float speed, float lifetime)
        {
            _speed = speed;
            _lifetime = lifetime;
        }

        private void Update()
        {
            _passedLifetime += Time.deltaTime;
            if (_passedLifetime >= _lifetime)
            {
                Destroy(gameObject);
                return;
            }

            var command = new TranslateGameFieldObjectCommand(
                gameObject,
                Vector3.forward * Time.deltaTime * _speed);
            AsyncMessageBus.Default.Publish(command);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Asteroid>() != null)
            {
                Destroy(other.gameObject);
                Destroy(gameObject);
            }
        }
    }
}