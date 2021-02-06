using AsteraX.GameSimulation.Commands;
using MediatR;
using UnityEngine;

namespace AsteraX.GameSimulation.Player.Bullets
{
    public class Bullet : MonoBehaviour
    {
        private float _speed;
        private float _lifetime;
        private float _passedLifetime;
        private ISender _sender;

        public void Initialize(float speed, float lifetime, ISender sender)
        {
            _speed = speed;
            _lifetime = lifetime;
            _sender = sender;
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
            _sender.Send(command);
        }
    }
}