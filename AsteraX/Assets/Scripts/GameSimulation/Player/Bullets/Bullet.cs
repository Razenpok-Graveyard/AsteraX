using AsteraX.GameSimulation.Commands;
using UnityEngine;
using static MediatR.MediatorSingleton;

namespace AsteraX.GameSimulation.Player.Bullets
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
            Send(command);
        }
    }
}