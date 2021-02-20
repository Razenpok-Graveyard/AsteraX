using AsteraX.Application.Game.Commands;
using UniTaskPubSub;
using UnityEngine;

namespace AsteraX.Application.Game.Player
{
    public class PlayerShipMovementPresenter : MonoBehaviour
    {
        [SerializeField] private Transform _ship;
        [SerializeField] private PlayerShipSettings _settings;

        private void Awake()
        {
            this.Subscribe<MoveShipInputNotification>(Handle);
        }

        private void Handle(MoveShipInputNotification notification)
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            var movement = notification.Movement;
            var translationVector = new Vector2(movement.x, movement.y);
            var translation = translationVector * _settings.MaximumSpeed * Time.deltaTime;
            AsyncMessageBus.Default.Publish(new TranslateGameFieldObjectCommand(gameObject, translation));

            var rotationVector = new Vector2(movement.y, -movement.x);
            var rotation = rotationVector * _settings.MaximumTilt;
            _ship.localRotation = Quaternion.Euler(rotation.x, rotation.y, 0);
        }
    }
}