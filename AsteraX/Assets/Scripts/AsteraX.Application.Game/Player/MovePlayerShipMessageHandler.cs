using UnityEngine;

namespace AsteraX.Application.Game.Player
{
    public class MovePlayerShipMessageHandler : MonoBehaviour
    {
        [SerializeField] private Transform _ship;
        [SerializeField] private PlayerShipSettings _settings;

        private void Awake()
        {
            this.Subscribe<MovePlayerShip>(Handle);
        }

        private void Handle(MovePlayerShip notification)
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            var movement = notification.Movement;
            var translationVector = new Vector2(movement.x, movement.y);
            var rigidBody = _ship.GetComponent<Rigidbody>();
            rigidBody.velocity = translationVector * _settings.MaximumSpeed;

            var rotationVector = new Vector2(movement.y, -movement.x);
            var rotation = rotationVector * _settings.MaximumTilt;
            _ship.localRotation = Quaternion.Euler(rotation.x, rotation.y, 0);
        }
    }
}