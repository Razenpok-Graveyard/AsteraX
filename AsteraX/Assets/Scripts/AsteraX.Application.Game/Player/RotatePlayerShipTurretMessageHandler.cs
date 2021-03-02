using AsteraX.Application.Common;
using UnityEngine;

namespace AsteraX.Application.Game.Player
{
    public class RotatePlayerShipTurretMessageHandler : MonoBehaviour
    {
        [SerializeField] private Transform _turret;

        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
            this.Subscribe<RotatePlayerShipTurret>(Handle);
        }

        private void Handle(RotatePlayerShipTurret message)
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            var positionOnScreen = _mainCamera.WorldToViewportPoint(_turret.position);
            var angle = AngleBetweenPoints(positionOnScreen, message.ScreenPosition);
            var rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
            // Offset because of model
            _turret.rotation = rotation * Quaternion.Euler(0, 0, 90);
        }

        private static float AngleBetweenPoints(Vector2 a, Vector2 b)
        {
            return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
        }
    }
}